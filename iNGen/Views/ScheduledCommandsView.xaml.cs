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
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Diagnostics;
using iNGen.Models;

namespace iNGen.Views
{
    public partial class ScheduledCommandsView : UserControl
    {
        public ScheduledCommandsViewModel ViewModel { get; set; }
        private MetroWindow View;

        public ScheduledCommandsView()
        {
            ViewModel = new ScheduledCommandsViewModel();
            InitializeComponent();
            Loaded += ScheduledCommandsView_Loaded;
        }

        void ScheduledCommandsView_Loaded(object sender, RoutedEventArgs e)
        {
            View = App.Current.MainWindow as MainWindow;
        }

        private async void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (ScheduledTaskListView.SelectedIndex == -1)
            {
                await ShowMessageDialog("You must select an item to remove from the Tasks List");
                return;
            }
            ViewModel.ScheduledTasks.Tasks.RemoveAt(ScheduledTaskListView.SelectedIndex);
        }

        private void AddTaskBUtton_Click(object sender, RoutedEventArgs e)
        {
            var flyout = View.Flyouts.Items[0] as Flyout;
            flyout.IsOpen = true;
        }

        private async Task<MessageDialogResult> ShowConfirmDialog(string dialogText)
        {
            View.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
            };
            return await View.ShowMessageAsync("Question", dialogText, MessageDialogStyle.AffirmativeAndNegative, mySettings);
        }

        private async Task<MessageDialogResult> ShowMessageDialog(string dialogText)
        {
            View.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
            };
            return await View.ShowMessageAsync("Information", dialogText, MessageDialogStyle.Affirmative, mySettings);
        }

        private void ToggleTask_Checked(object sender, RoutedEventArgs e)
        {
            ScheduledTaskListView.SelectedIndex = -1;
            var toggleSlider = (ToggleSwitch)sender;
            var parent = FindParent<ListViewItem>(toggleSlider);
            var item = parent.Content as ScheduledTask;
            Helpers.TaskCreationDestruction.CancelTask(item);
            Helpers.TaskCreationDestruction.CreateTask(item);
        }

        public T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private void ToggleTask_Unchecked(object sender, RoutedEventArgs e)
        {
            ScheduledTaskListView.SelectedIndex = -1;
            var toggleSlider = (ToggleSwitch)sender;
            var parent = FindParent<ListViewItem>(toggleSlider);
            var item = parent.Content as ScheduledTask;
            item.IsEnabled = false;
            Helpers.TaskCreationDestruction.CancelTask(item);
        }
    }
}
