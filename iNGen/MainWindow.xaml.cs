using iNGen.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PTK.Extensions;
using System.IO;
using MahApps.Metro.Controls;
using Squirrel;
using System.Xml.Linq;
using MahApps.Metro.Controls.Dialogs;
using GalaSoft.MvvmLight.Messaging;

namespace iNGen
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            Messenger.Default.Register<NotificationMessage<string>>(this, NotificationStringRecieved);
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
            Messenger.Default.Unregister<NotificationMessage<string>>(this);
        }

        private async void NotificationStringRecieved(NotificationMessage<string> message)
        {
            switch (message.Notification)
            {
                case "ShowMessageDialog":
                    await ShowMessageDialog(message.Content);
                    break;
                case "ShowConfirmDeleteServerDialog":
                    var result = await ShowConfirmDialog(message.Content);
                    Messenger.Default.Send(new NotificationMessage<MessageDialogResult>(result, "ConfirmDeleteServerDialogResult"));
                    break;
                default:
                    return;
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CheckForUpdates();
            StartUpdateTimer();
        }

        private void NotificationMessageReceived(NotificationMessage message)
        {
            switch (message.Notification)
            {
                case "ShowAboutWindow":
                    ShowAboutWindow();
                    break;
                default:
                    return;
            }
        }

        private void StartUpdateTimer()
        {
            var _updateTimer = new System.Timers.Timer() { Interval = 60 * 60 * 1000, Enabled = true, AutoReset = true };
            _updateTimer.Elapsed += timer_Elapsed;
            _updateTimer.Start();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckForUpdates();
        }

        private async void CheckForUpdates()
        {
            using (var mgr = new UpdateManager("http://arkmanager.teamitf.co.uk/iNGen/Releases/", "iNGen"))
            {
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v => mgr.CreateShortcutForThisExe(),
                    onAppUpdate: v => mgr.CreateShortcutForThisExe(),
                    onAppUninstall: v => mgr.RemoveShortcutForThisExe());

                try
                {
                    UpdateInfo updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.FutureReleaseEntry != null)
                    {
                        if (updateInfo.CurrentlyInstalledVersion != null)
                        {
                            XElement xelement = XElement.Load("http://arkmanager.teamitf.co.uk/iNGen/version.xml");
                            StringReader reader = new StringReader(xelement.ToString());
                            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.AppUpdates));
                            Models.AppUpdates appUpdates = (Models.AppUpdates)xmlSerializer.Deserialize(reader);
                            string changes = MakeChangeLog(appUpdates);
                            if (updateInfo.CurrentlyInstalledVersion.Version == updateInfo.FutureReleaseEntry.Version) return;
                            var updateDialog = new Views.AppUpdate(updateInfo, changes) { Owner = this };
                            var result = updateDialog.ShowDialog();
                            if (result == false) return;
                            await mgr.UpdateApp();
                            var oldPath = System.IO.Path.Combine(mgr.RootAppDirectory, "app-" + updateInfo.CurrentlyInstalledVersion.Version.ToString(), "UserData");
                            var newPath = System.IO.Path.Combine(mgr.RootAppDirectory, "app-" + updateInfo.FutureReleaseEntry.Version.ToString(), "UserData");
                            DirectoryInfo d = new DirectoryInfo(oldPath);
                            var files = d.GetFiles();
                            foreach (var file in files)
                            {
                                file.CopyTo(System.IO.Path.Combine(newPath, file.Name), true);
                            }

                            MessageBox.Show("iNGen Has been Updated. Please Re-Launch It.");
                            Application.Current.Shutdown(0);
                        }
                        else
                        {
                            await mgr.UpdateApp();
                            MessageBox.Show("iNGen Has been Updated. Please Re-Launch It.");
                            Application.Current.Shutdown(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Checking for Updates Failed: " + ex.Message);
                }
            }
        }

        private string MakeChangeLog(Models.AppUpdates application)
        {
            StringBuilder changelog = new StringBuilder();
            changelog.Append("<h1><strong>Application Change Log</strong></h1>");
            var logs = application.AppUpdate.OrderByDescending(a => a.Version);
            foreach (var item in logs)
            {
                changelog.Append("<p>&nbsp;</p>");
                changelog.AppendFormat("<h3><strong>Version: {0}</strong></h3>", item.Version);
                changelog.Append(item.ChangeNotes);
            }
            return changelog.ToString();
        }

        private void ApplicationClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (App.ModelManager.Get<UserSettings>().GeneralSettings.CloseToSystemTray)
            {
                e.Cancel = true;
                Hide();
            }

            App.ModelManager.SaveAll();
            App.LogManager.SaveLog();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            App.ArkRcon.Run();
            MainLoop();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.StopFlashing();
        }

        private async Task MainLoop()
        {
            App.ArkRcon.ServerConnectionSucceeded += (s, args) =>
                {
                    foreach (var timedEvent in App.EventManager.Events)
                    {
                        timedEvent.Reset();
                    }
                };

            while (true)
            {
                App.EventManager.Update();
                await Task.Delay(1000);
            }
        }

        private async Task<MessageDialogResult> ShowMessageDialog(string dialogText)
        {
            this.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
            };
            return await this.ShowMessageAsync("Information", dialogText, MessageDialogStyle.Affirmative, mySettings);
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                if (App.ModelManager.Get<UserSettings>().GeneralSettings.MinimizeToSystemTray)
                {
                    this.Hide();
                }
            }
        }

        private void MetroWindow_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = App.ModelManager.Get<UserSettings>().GeneralSettings.KeepWindowTopmost;
        }

        private async void ShowAboutWindow()
        {
            using (var mgr = new UpdateManager("http://arkmanager.teamitf.co.uk/iNGen/Releases/", "iNGen"))
            {
                try
                {
                    UpdateInfo updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.FutureReleaseEntry != null)
                    {
                        if (updateInfo.CurrentlyInstalledVersion != null)
                        {
                            XElement xelement = XElement.Load("http://arkmanager.teamitf.co.uk/iNGen/version.xml");
                            StringReader reader = new StringReader(xelement.ToString());
                            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.AppUpdates));
                            Models.AppUpdates appUpdates = (Models.AppUpdates)xmlSerializer.Deserialize(reader);
                            string changes = MakeChangeLog(appUpdates);
                            var updateDialog = new Views.AboutApp(updateInfo, changes) { Owner = this };
                            var result = updateDialog.ShowDialog();
                            if (result == false) return;
                            CheckForUpdates();
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
        }

       

        private void CommandTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DelayForm.Visibility = System.Windows.Visibility.Collapsed;
            BroadcastForm.Visibility = Visibility.Collapsed;
            CustomForm.Visibility = Visibility.Collapsed;
            NewTaskBroadcastVariable.Text = "";
            NewTaskCustomVariable.Text = "";
            NewTaskDelayVariable.Text = "";
            var combo = (ComboBox)sender;
            if (combo.SelectedIndex == -1) return;
            switch(combo.SelectedIndex)
            {
                case 0:
                    DelayForm.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 1:
                    BroadcastForm.Visibility = Visibility.Visible;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    CustomForm.Visibility = Visibility.Visible;
                    break;
                default:
                    break;

            }
        }

        private async Task<MessageDialogResult> ShowConfirmDialog(string dialogText)
        {
            MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
            };
            return await this.ShowMessageAsync("Question", dialogText, MessageDialogStyle.AffirmativeAndNegative, mySettings);
        }

    }
}
