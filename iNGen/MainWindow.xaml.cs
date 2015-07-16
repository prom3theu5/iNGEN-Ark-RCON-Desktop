using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO;
using MahApps.Metro.Controls;
using Squirrel;
using System.Xml.Linq;
using System.Xml.Serialization;
using iNGen.Views;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro;
using System.ComponentModel;

namespace iNGen
{
    public partial class MainWindow : MetroWindow
    {
        public GeneralSettings GeneralSettings { get; set; }
        public List<TaskCommand> TaskList { get; set; }
        
        
        public MainWindow()
        {
            GeneralSettings = App.ModelManager.Get<UserSettings>().GeneralSettings;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            TaskList = new List<TaskCommand>();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CommandTypeCombo.ItemsSource = Enum.GetValues(typeof(CommandType)).Cast<Enum>().Select(value => new
        {
            (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute).Description,
            value
        })
        .OrderBy(item => item.value)
        .Select(item => new TaskCommands
        {
            Description = item.Description,
            value = item.value
        })
        .ToList();
            CheckForUpdates();
            StartUpdateTimer();
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
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Models.AppUpdates));
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

        private void CancelAddNewTask_Click(object sender, RoutedEventArgs e)
        {
            var flyout = this.Flyouts.Items[0] as Flyout;
            flyout.IsOpen = false;
            TaskCommandList.ItemsSource = null;
            TaskList.Clear();
            TaskNameBox.Text = "";
            TaskRepeatableCheckbox.IsChecked = false;
            TaskIntervalBox.Text = "1";

        }

        private async void ConfirmAddNewTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskNameBox.Text))
            {
                await ShowMessageDialog("Please enter a Task Name!");
                return;
            }
            var scheduledTask = NavigationViewObject.ViewModel.NavigationItems.FirstOrDefault(a => a.Header == "Scheduled Tasks");
            if (scheduledTask == null) CancelAddNewTask_Click(sender, e);
            var item = scheduledTask.Content as ScheduledCommandsView;
            int output;
            var result = int.TryParse(TaskIntervalBox.Text, out output);
            string units;
            switch(TaskIntervalPeriodVariable.SelectedIndex)
            {
                case 0:
                    units = "Second(s)";
                    break;
                case 1:
                    output = output * 60;
                    units = "Minute(s)";
                    break;
                case 2:
                    output = output * 3600;
                    units = "Hour(s)";
                    break;
                case 3:
                    output = output * 86400;
                    units = "Day(s)";
                    break;
                default:
                    return;
            }
            if (result == false)
            {
                await ShowMessageDialog("Repeat Interval must be a whole number (integer).");
                return;
            }
            var task = new ScheduledTask
            {
                TaskName = TaskNameBox.Text,
                IsRepeat = (bool)TaskRepeatableCheckbox.IsChecked,
                RepeatInterval = output,
                IntervalUnit = units,
                IsEnabled = (bool)TaskEnabledCheckbox.IsChecked
            };
            task.TaskCommands = new List<TaskCommand>();
            foreach (TaskCommand command in TaskList)
            {
                task.TaskCommands.Add(command);
            }
            item.ViewModel.ScheduledTasks.Tasks.Add(task);
            CancelAddNewTask_Click(sender, e);
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

        private async void ShowAboutApp_Click(object sender, RoutedEventArgs e)
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
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Models.AppUpdates));
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

        private void DeleteTaskCommand_Click(object sender, RoutedEventArgs e)
        {
            if (TaskCommandList.SelectedIndex == -1) return;
            var index = TaskCommandList.SelectedIndex;
            TaskCommandList.ItemsSource = null;
            TaskList.RemoveAt(index);
            TaskCommandList.ItemsSource = TaskList;
        }

        private void AddTaskCommand_Click(object sender, RoutedEventArgs e)
        {
            AddTaskCommandWindow.IsOpen = true;
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

        private async void ConfirmAddTaskCommand_Click(object sender, RoutedEventArgs e)
        {
            if (CommandTypeCombo.SelectedIndex == -1) return;
            TaskCommandList.ItemsSource = null;
            var item = CommandTypeCombo.SelectedItem as TaskCommands;
            TaskCommand command = new TaskCommand();
            command.CommandType = (CommandType)item.value;
            switch(CommandTypeCombo.SelectedIndex)
            {
                case 0:
                    if (string.IsNullOrWhiteSpace(NewTaskDelayVariable.Text)) return;
                    int output;
                    bool result = int.TryParse(NewTaskDelayVariable.Text, out output);
                    if (!result)
                    {
                        await ShowMessageDialog("You must enter a whole number (Integer");
                        return;
                    }
                    switch (NewTaskDelayPeriodVariable.SelectedIndex)
                    {
                        case 0:
                            command.ExtraInfo = "Seconds";
                            break;
                        case 1:
                            output = output * 60;
                            command.ExtraInfo = "Minutes";
                            break;
                        case 2:
                            output = output * 3600;
                            command.ExtraInfo = "Hours";
                            break;
                        case 3:
                            output = output * 86400;
                            command.ExtraInfo = "Days";
                            break;
                        default:
                            return;
                    }
                    command.Variable = output.ToString();
                    break;
                case 1:
                    if (string.IsNullOrWhiteSpace(NewTaskBroadcastVariable.Text)) return;
                    command.Variable = NewTaskBroadcastVariable.Text;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    if (string.IsNullOrWhiteSpace(NewTaskCustomVariable.Text)) return;
                    command.Variable = NewTaskCustomVariable.Text;
                    break;
                default:
                    return;
            }
            TaskList.Add(command);
            TaskCommandList.ItemsSource = TaskList;
            CancelAddTaskCommand_Click(sender, e);
        }

        private void CancelAddTaskCommand_Click(object sender, RoutedEventArgs e)
        {
            CommandTypeCombo.SelectedIndex = -1;
            NewTaskDelayPeriodVariable.SelectedIndex = 0;
            AddTaskCommandWindow.IsOpen = false;
        }
    }
}
