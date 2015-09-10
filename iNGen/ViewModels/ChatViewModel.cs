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
using System.Windows;
using System.Windows.Controls.Primitives;
using PTK.Utils;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using iNGen.Views;
using iNGen.Helpers;
using iNGen.Views.Popups;
using PTK.Extensions;

namespace iNGen.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ChatSettings ChatSettings { get; set; }
        public ChatView View { get; set; }
        public FixedSizeObservableCollection<ChatMessage> ChatMessages { get; set; }
        public bool EnableChat { get; set; }
        public bool DisableChat { get; set; }
        public int NewMessageLength => NewMessage.Length;
        private Task _getChatMessagesTask;
        private CancellationTokenSource _cancellationToken;
        public RelayCommand ClearChatCommand { get; set; }
        public RelayCommand EnableChatCommand { get; set; }
        public RelayCommand DisableChatCommand { get; set; }
        public RelayCommand NewMessageFocusChangeCommand { get; set; }

        public RelayCommand NewMessageEnterKeyCommand { get; set; }
        public string NewMessage { get; set; }
        private MessageSide _curside;

        public ChatViewModel()
        {
            ChatSettings = App.ModelManager.Get<UserSettings>().ChatSettings;
            ChatMessages = new FixedSizeObservableCollection<ChatMessage>(30);
            EnableChat = true;
            DisableChat = false;
            NewMessage = string.Empty;

            #region Rcon Events
            App.ArkRcon.ChatLogUpdated += (s, args) =>
            {
                AddTextRecieved(args);
            };

            App.ArkRcon.SentMessageUpdated += (s, args) =>
            {
                AddTextSend(args);
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
            NewMessageEnterKeyCommand = new RelayCommand(NewMessageEnterKeyPress);
            NewMessageFocusChangeCommand = new RelayCommand(NewMessageFocusChange);
        }

        private void NewMessageFocusChange()
        {
            if (ChatSettings.IsAutoScrollEnabled)
                View.ScrollConversationToEnd();
        }

        private void NewMessageEnterKeyPress()
        {
            if (string.IsNullOrWhiteSpace(NewMessage)) return;
            App.ArkRcon.Say(NewMessage, ChatSettings.IsCustomServerConsoleNameEnabled ? ChatSettings.CustomServerConsoleName : null);

            NewMessage = string.Empty;
            if (ChatSettings.IsAutoScrollEnabled)
            {
                View.ScrollConversationToEnd();
            }
            View.TextInput.Focus();
        }

        private void DoDisableChat()
        {
            DisableChat = false;
            EnableChat = true;
            _cancellationToken?.Cancel();
            _getChatMessagesTask?.Wait();
        }

        private void DoEnableChat()
        {
            EnableChat = false;
            DisableChat = true;
            _cancellationToken = new CancellationTokenSource();
            _getChatMessagesTask = Repeat.Interval(TimeSpan.FromSeconds(3), GetChatMessagesTask, _cancellationToken.Token, true);
        }

        private void ClearChat()
        {
            ChatMessages.Clear();
        }

        //Craptastic fudged in Boolean "GotChatResponse" to see if the server is still responding to requests. They better fix this crap soon.....
        private static async void GetChatMessagesTask()
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
            {
                App.ArkRcon.ExecCommand(Ark.Opcode.ChatMessage, "getchat");
                await Task.Delay(3000);
                if (App.ArkRcon.GotChatResponse) return;
                App.ArkRcon.Disconnect();
                await App.ArkRcon.Connect(App.Locator.Home.SelectedServer);
            }));
        }

        private void AddTextSend(ChatLogEventArgs args)
        {
            var message = new ChatMessage { PrevSide = _curside };
            if (ChatSettings.IsCustomServerConsoleNameEnabled)
                message.SenderName = string.IsNullOrWhiteSpace(ChatSettings.CustomServerConsoleName) ? "Server Admin" : ChatSettings.CustomServerConsoleName;
            else
                message.SenderName = "Server Admin";
            message.Text = args.Message;
            message.Side = MessageSide.Me;
            if (ChatSettings.IsTimestampingEnabled)
                message.Timestamp = args.Timestamp;
            ChatMessages.Add(message);
            _curside = MessageSide.Me;
            if (ChatSettings.IsAutoScrollEnabled)
            {
                View.ScrollConversationToEnd();
            }
        }

        private void AddTextRecieved(ChatLogEventArgs args)
        {
            var message = new ChatMessage
            {
                PrevSide = _curside,
                SenderName = args.Sender,
                Text = args.Message,
                Side = MessageSide.You
            };

            if (ChatSettings.IsTimestampingEnabled)
                message.Timestamp = args.Timestamp;

            ChatMessages.Add(message);
            _curside = MessageSide.You;
            if (ChatSettings.IsAutoScrollEnabled)
            {
                View.ScrollConversationToEnd();
            }

            try
            {
                if (!ChatSettings.IsNotificationsEnabled || ChatSettings.NotificationString == null) return;

                if (ChatSettings.NotificationWords.Any(s => message.Text.Contains(s)))
                {
                    ShowCustomBalloon($"{message.SenderName}: {message.Text}");
                }

                if (ChatSettings.IsFlashWindowNotificationEnabled)
                    Application.Current.MainWindow.FlashWindow();

                if (!ChatSettings.IsNotificationSoundFileEnabled || ChatSettings.NotificationSoundFile == null) return;
                    SoundManager.PlayFile(ChatSettings.NotificationSoundFile);
            }
            catch (Exception ex)
            {
                App.LogErrorMessage(ex);
            }
        }

        private static void ShowCustomBalloon(string message)
        {
            App.Locator.Notification.NotificationCaption = "Chat Notification";
            App.Locator.Notification.NotificationMessage = message;
            var balloon = new NotificationPopup();
            App.Tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);
        }
    }
}
