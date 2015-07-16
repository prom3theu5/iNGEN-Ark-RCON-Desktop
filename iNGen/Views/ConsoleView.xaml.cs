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
using PTK.Utils;

namespace iNGen.Views
{
    public partial class ConsoleView : UserControl
    {
        public ConsoleViewModel ViewModel {get; set;}

        public ConsoleView()
        {
            ViewModel = new ConsoleViewModel();
            InitializeComponent();
            App.ArkRcon.ConsoleLogUpdated += (s, args) =>
            {
                if (ViewModel.ConsoleSettings.IsAutoScrollEnabled)
                    ChatScrollViewer.ScrollToBottom();
            };

            App.ArkRcon.ConsoleLogUpdated += (s, args) =>
            {
                string message = args.Message;
                if (ViewModel.ConsoleSettings.IsTimestampingEnabled)
                    message = args.Timestamp.ToString("(hh:mm tt) ") + message;
                var run = new Run(message);
                if (ViewModel.ConsoleSettings.IsNotificationsEnabled && ViewModel.ConsoleSettings.NotificationWords != null)
                {
                    foreach (var notificationWord in ViewModel.ConsoleSettings.NotificationWords)
                    {
                        if (message.Contains(notificationWord))
                        {
                            run.Foreground = new SolidColorBrush(Color.FromRgb(68, 138, 255));

                            if (ViewModel.ConsoleSettings.IsFlashWindowNotificationEnabled)
                            {
                                Application.Current.MainWindow.FlashWindow();
                            }
                            break;
                        }
                    }
                }
                ChatTextBox.Document.Blocks.Add(new Paragraph(run));
            };

        }

        private void ChatBoxKeyDown(object sender, KeyEventArgs e)
        {
            var textbox = sender as TextBox;

            if(textbox != null)
            {
                if(e.Key == Key.Enter)
                {
                    if(ViewModel.ConsoleSettings.IsCustomServerConsoleNameEnabled)
                        App.ArkRcon.ConsoleCommand(textbox.Text, ViewModel.ConsoleSettings.CustomServerConsoleName);
                    else
                        App.ArkRcon.ConsoleCommand(textbox.Text, null);

                    textbox.Clear();
                }
            }
        }

        private void ClearChatButtonClick(object sender, RoutedEventArgs e)
        {
            ChatTextBox.Document.Blocks.Clear();
        }
    }
}
