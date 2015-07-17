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
using GalaSoft.MvvmLight.Messaging;

namespace iNGen.Views
{
    public partial class ScheduledCommandsView : UserControl
    {
        public ScheduledCommandsViewModel ViewModel { get { return DataContext as ScheduledCommandsViewModel; } }
        
        public ScheduledCommandsView()
        {
            InitializeComponent();
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
