using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels
{
    public class ScheduledCommandsViewModel : ViewModelBase
    {
        public ScheduledTaskSet ScheduledTasks { get; set; }
        public ScheduledTask SelectedScheduledTask { get; set; }
        public RelayCommand AddTaskCommand { get; set; }
        public RelayCommand DeleteTaskCommand { get; set; }
        
        public ScheduledCommandsViewModel()
        {
            AddTaskCommand = new RelayCommand(AddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            ScheduledTasks = new ScheduledTaskSet();
            LoadTasks();
        }

        private void DeleteTask()
        {
            if (SelectedScheduledTask == null)
            {
                Messenger.Default.Send(new NotificationMessage<string>("Please select an item from the list to remove.", "ShowMessageDialog"));
                return;
            }
            ScheduledTasks.Tasks.Remove(SelectedScheduledTask);
        }

        private void AddTask()
        {
            Messenger.Default.Send(new NotificationMessage("OpenAddNewTask"));
        }

        private void LoadTasks()
        {
            try
            {
                ScheduledTasks = App.ModelManager.Get<ScheduledTaskSet>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

        
        

    }
}
