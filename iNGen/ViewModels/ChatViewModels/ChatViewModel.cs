using PTK.WPF;
using iNGen.Models;
using Ark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Threading;
using PTK.Utils;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace iNGen.ViewModels.ChatViewModels
{
    public class ChatViewModel: ViewModelBase
    {
        public ChatSettings ChatSettings {get; set;}
        public ObservableCollection<ChatMessage> ChatMessages { get; set; }
        public bool EnableChat { get; set; }
        public bool DisableChat { get; set; }
        private Task _getChatMessagesTask;
        private CancellationTokenSource _cancellationToken;
        public RelayCommand ClearChatCommand { get; set; }
        public RelayCommand EnableChatCommand { get; set; }
        public RelayCommand DisableChatCommand { get; set; }
        public string NewMessage { get; set; }
        private MessageSide curside;

        public ChatViewModel()
        {
            ChatSettings = App.ModelManager.Get<UserSettings>().ChatSettings;
            ChatMessages = new ObservableCollection<ChatMessage>();
            EnableChat = true;
            DisableChat = false;

            #region Rcon Events
            App.ArkRcon.ChatLogUpdated += (s, args) =>
            {
                    addTextRecieved(args);
            };

            App.ArkRcon.SentMessageUpdated += (s, args) =>
            {
                    addTextSend(args);
            };

            App.ArkRcon.ServerAuthSucceeded += (s, args) =>
            {
                if (App.ModelManager.Get<UserSettings>().GeneralSettings.AutoStartChat)
                   DoEnableChat();
            };

            App.ArkRcon.ServerConnectionDisconnected += (s, args) =>
            {
                DoDisableChat();
            };
            #endregion

            ClearChatCommand = new RelayCommand(ClearChat);
            EnableChatCommand = new RelayCommand(DoEnableChat);
            DisableChatCommand = new RelayCommand(DoDisableChat);

            Messenger.Default.Register<NotificationMessage>(this, OnSendMessage);
        }

        private void DoDisableChat()
        {
            DisableChat = false;
            EnableChat = true;
            if (_cancellationToken != null) _cancellationToken.Cancel();
            if (_getChatMessagesTask != null)
                _getChatMessagesTask.Wait();
        }

        private void DoEnableChat()
        {
            EnableChat = false;
            DisableChat = true;
            _cancellationToken = new CancellationTokenSource();
            _getChatMessagesTask = Repeat.Interval(TimeSpan.FromSeconds(3), () => GetChatMessagesTask(), _cancellationToken.Token, true);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Messenger.Default.Unregister<NotificationMessage>(this);
        }

        private void OnSendMessage(NotificationMessage message)
        {
            switch (message.Notification)
            {
                case "SendAChatMessage":
                    if (string.IsNullOrWhiteSpace(NewMessage)) return;
                    if (ChatSettings.IsCustomServerConsoleNameEnabled)
                       App.ArkRcon.Say(NewMessage, ChatSettings.CustomServerConsoleName);
                    else
                       App.ArkRcon.Say(NewMessage, null);
                    Messenger.Default.Send(new NotificationMessage("SentNewMessage"));
                    break;
                default:
                    return;
            }
        }

        private void ClearChat()
        {
            ChatMessages.Clear();
        }

        private void GetChatMessagesTask()
        {
            App.ArkRcon.ExecCommand(Ark.Opcode.ChatMessage, "getchat");
        }

        private void addTextSend(ChatLogEventArgs args)
        {
            var message = new ChatMessage();
            message.PrevSide = curside;
            if (ChatSettings.IsCustomServerConsoleNameEnabled)
                message.SenderName = string.IsNullOrWhiteSpace(ChatSettings.CustomServerConsoleName) ? "Server Admin" : ChatSettings.CustomServerConsoleName;
            else
                message.SenderName = "Server Admin";
            message.Text = args.Message;
            message.Side = MessageSide.Me;
            if (ChatSettings.IsTimestampingEnabled)
                message.Timestamp = args.Timestamp;
            ChatMessages.Add(message);
            curside = MessageSide.Me;
        }

        private void addTextRecieved(ChatLogEventArgs args)
        {
            var message = new ChatMessage();
            message.PrevSide = curside;
            message.SenderName = args.Sender;
            message.Text = args.Message;
            message.Side = MessageSide.You;

            if (ChatSettings.IsTimestampingEnabled)
                message.Timestamp = args.Timestamp;

            ChatMessages.Add(message);
            curside = MessageSide.You;
            Messenger.Default.Send(new NotificationMessage("ScrollChatToEnd"));
        }

        
    }
}