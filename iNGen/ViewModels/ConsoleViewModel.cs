using PTK.WPF;
using iNGen.Models;
using Ark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iNGen.Views;
using iNGen.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace iNGen.ViewModels
{
    public class ConsoleViewModel: ViewModelBase
    {
        public ConsoleSettings ConsoleSettings {get; set;}
        public FixedSizeObservableCollection<string> ConsoleMessages { get; set; }
        public string NewCommand { get; set; }
        public ConsoleView View { get; set; }
        public RelayCommand ChatBoxKeyDown { get;set;}
        public RelayCommand ClearConsoleCommand { get; set; }

        public ConsoleViewModel()
        {
            ConsoleSettings = App.ModelManager.Get<UserSettings>().ConsoleSettings;
            ConsoleMessages = new FixedSizeObservableCollection<string>(100);
            NewCommand = string.Empty;
            ChatBoxKeyDown = new RelayCommand(ConsoleCommandEnter);
            ClearConsoleCommand = new RelayCommand(ClearConsole);

            App.ArkRcon.ConsoleLogUpdated += (s, args) =>
            {
                string message = args.Message;
                if (ConsoleSettings.IsTimestampingEnabled)
                    message = args.Timestamp.ToString("(hh:mm tt) ") + message;
                    ConsoleMessages.Add(message);
                    CheckScrollToBottom();
            };
        }

        private void CheckScrollToBottom()
        {
            if (ConsoleSettings.IsAutoScrollEnabled)
            {
                View.ChatScrollViewer.ScrollToBottom();
            }
        }

        private void ClearConsole()
        {
            ConsoleMessages.Clear();
        }

        private void ConsoleCommandEnter()
        {
            if (!string.IsNullOrWhiteSpace(NewCommand))
            {
                if (ConsoleSettings.IsCustomServerConsoleNameEnabled)
                    App.ArkRcon.ConsoleCommand(NewCommand, ConsoleSettings.CustomServerConsoleName);
                else
                    App.ArkRcon.ConsoleCommand(NewCommand, null);
            }
            NewCommand = string.Empty;
            CheckScrollToBottom();
        }
    }
}