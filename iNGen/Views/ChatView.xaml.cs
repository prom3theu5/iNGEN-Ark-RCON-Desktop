using iNGen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PTK.Extensions;
using System.Threading;
using PTK.Utils;
using iNGen.Models;

namespace iNGen.Views
{
    public partial class ChatView : UserControl
    {
        public ChatViewModel ViewModel {get; set;}
        private Task _getChatMessagesTask;
        private CancellationTokenSource _cancellationToken;

        public ChatView()
        {
            ViewModel = new ChatViewModel();


            App.ArkRcon.ChatLogUpdated += (s, args) =>
                {
                    if (ViewModel.ChatSettings.IsAutoScrollEnabled)
                        ChatScrollViewer.ScrollToBottom();
                };

            App.ArkRcon.ChatLogUpdated += (s, args) =>
                {
                    string[] messages = args.Message.Split('\n');
                    foreach (string message in messages)
                    {
                        if (string.IsNullOrWhiteSpace(message)) continue;
                        string outputmessage = message;
                        if (ViewModel.ChatSettings.IsTimestampingEnabled)
                            outputmessage = args.Timestamp.ToString("(hh:mm tt) ") + outputmessage;
                        var run = new Run(outputmessage);

                        if (args.IsAdmin)
                        {
                            run.Foreground = new SolidColorBrush(Colors.Blue);
                            run.FontWeight = FontWeights.Bold;
                        }

                        if (ViewModel.ChatSettings.IsNotificationsEnabled && ViewModel.ChatSettings.NotificationWords != null)
                        {
                            foreach (var notificationWord in ViewModel.ChatSettings.NotificationWords)
                            {
                                if (outputmessage.Contains(notificationWord))
                                {
                                    run.Foreground = new SolidColorBrush(Color.FromRgb(68, 138, 255));

                                    if (ViewModel.ChatSettings.IsFlashWindowNotificationEnabled)
                                    {
                                        Application.Current.MainWindow.FlashWindow();
                                    }
                                    break;
                                }
                            }
                        }
                        FilterSpam(run);
                    }
                };

            InitializeComponent();

            App.ArkRcon.ServerAuthSucceeded += (s, args) =>
            {
                if (App.ModelManager.Get<UserSettings>().GeneralSettings.AutoStartChat)
                {
                    if (!App.ArkRcon.IsConnected) return;
                    EnableChatBUtton.IsEnabled = false;
                    DisableChatBUtton.IsEnabled = true;
                    _cancellationToken = new CancellationTokenSource();
                    _getChatMessagesTask = Repeat.Interval(TimeSpan.FromSeconds(3), () => GetChatMessagesTask(), _cancellationToken.Token, true);
                }
            };

                App.ArkRcon.ServerConnectionDisconnected += (s, args) =>
                {
                    DisableChatBUtton.IsEnabled = false;
                    EnableChatBUtton.IsEnabled = true;
                    if (_cancellationToken != null) _cancellationToken.Cancel();
                    if (_getChatMessagesTask != null)
                        _getChatMessagesTask.Wait();
                };
        }

        private void FilterSpam(Run run)
        {
            var paragraph = new Paragraph(run);
            if (ChatTextBox.Document.Blocks.LastBlock == paragraph) return;
            ChatTextBox.Document.Blocks.Add(paragraph);
        }

        private void ChatBoxKeyDown(object sender, KeyEventArgs e)
        {
            var textbox = sender as TextBox;

            if(textbox != null)
            {
                if(e.Key == Key.Enter)
                {
                    if (string.IsNullOrWhiteSpace(textbox.Text) || string.IsNullOrEmpty(textbox.Text)) return;
                    if(ViewModel.ChatSettings.IsCustomServerConsoleNameEnabled)
                        App.ArkRcon.Say(textbox.Text, ViewModel.ChatSettings.CustomServerConsoleName);
                    else
                        App.ArkRcon.Say(textbox.Text, null);

                    textbox.Clear();
                }
            }
        }

        private void ClearChatButtonClick(object sender, RoutedEventArgs e)
        {
            ChatTextBox.Document.Blocks.Clear();
        }

        private void EnableChatBUtton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.ArkRcon.IsConnected) return;
            EnableChatBUtton.IsEnabled = false;
            DisableChatBUtton.IsEnabled = true;
            _cancellationToken = new CancellationTokenSource();
            _getChatMessagesTask = Repeat.Interval(TimeSpan.FromSeconds(2), () => GetChatMessagesTask(), _cancellationToken.Token, true);
        }

        private void GetChatMessagesTask()
        {
            App.ArkRcon.ExecCommand(Ark.Opcode.ChatMessage, "getchat");
        }
       

        private void DisableChatBUtton_Click(object sender, RoutedEventArgs e)
        {
            DisableChatBUtton.IsEnabled = false;
            EnableChatBUtton.IsEnabled = true;
            if (_cancellationToken != null) _cancellationToken.Cancel();
        }
    }
}
