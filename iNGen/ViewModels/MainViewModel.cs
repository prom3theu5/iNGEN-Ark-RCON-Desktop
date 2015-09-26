using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using iNGen.Models;
using iNGen.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public GeneralSettings GeneralSettings { get; set; }

        #region Scheduler Properties and Commands
        public ObservableCollection<TaskCommand> TaskList { get; set; }
        public TaskCommand NewTaskCommand { get; set; }
        public TaskCommand SelectedTaskCommand { get; set; }
        public IEnumerable<TaskCommands> CommandTypeList { get; set; }
        public TaskCommands SelectedCommandType { get; set; }
        public bool AddTaskCommandWindowVis { get; set; }
        public bool AddTaskWindowVis { get; set; }
        public bool SendPrivateMessageVis { get; set; }
        public string TaskIntervalPeriodVariable { get; set; }
        public string NewTaskDelayPeriodVariable { get; set; }
        public string NewTaskBroadcastVariable { get; set; }
        public int NewTaskDelayVariable { get; set; }
        public string NewTaskCustomVariable { get; set; }
        public ScheduledTask NewScheduledTask { get; set; }


        #region Button Commands
        public RelayCommand ConfirmAddNewTaskCommand { get; set; }
        public RelayCommand CancelAddNewTaskCommand { get; set; }
        public RelayCommand DoAddTaskCommand { get; set; }
        public RelayCommand DontAddTaskCommand { get; set; }
        public RelayCommand AddTaskCommand { get; set; }
        public RelayCommand ShowAboutCommand { get; set; }
        public RelayCommand DelTaskCommandCommand { get; set; }
        #endregion

        #endregion
        

        public MainViewModel()
        {
            GeneralSettings = App.ModelManager.Get<UserSettings>().GeneralSettings;

            #region Scheduler Related
            TaskList = new ObservableCollection<TaskCommand>();
            CommandTypeList = GetCMEnum();
            NewScheduledTask = new ScheduledTask();
            AddTaskCommandWindowVis = false;
            AddTaskWindowVis = false;
            SendPrivateMessageVis = false;
            ConfirmAddNewTaskCommand = new RelayCommand(ConfirmAddNewTask);
            CancelAddNewTaskCommand = new RelayCommand(CancelAddNewTask);
            DoAddTaskCommand = new RelayCommand(DoAddTask);
            DontAddTaskCommand = new RelayCommand(DontAddTask);
            ShowAboutCommand = new RelayCommand(ShowAbout);
            AddTaskCommand = new RelayCommand(AddTask);
            DelTaskCommandCommand = new RelayCommand(DelTaskCommand);
            #endregion
            
            Messenger.Default.Register<NotificationMessage>(this, OnNotificationMessage);
        }

        #region Scheduler Methods

        private void OnNotificationMessage(NotificationMessage message)
        {
            switch (message.Notification)
            {
                case "OpenAddNewTask":
                    AddTaskWindowVis = true;
                    break;
                case "ShowPMWindow":
                    SendPrivateMessageVis = true;
                    break;
                case "PMMessageSent":
                    SendPrivateMessageVis = false;
                    break;
                default:
                    return;
            }
        }

       
        private void DontAddTask()
        {
            TaskIntervalPeriodVariable = string.Empty;
            NewTaskDelayPeriodVariable = string.Empty;
            NewTaskBroadcastVariable = string.Empty;
            NewTaskDelayVariable = 0;
            NewTaskCustomVariable = string.Empty;
            AddTaskCommandWindowVis = false;
            SelectedCommandType = null;
            SelectedTaskCommand = null;
        }

        private void DoAddTask()
        {
            if (SelectedCommandType == null)
            {
                Messenger.Default.Send(new NotificationMessage<string>("Please select the Task to Add first!", "ShowMessageDialog"));
                return;
                
            }

            switch (SelectedCommandType.Description)
            {
                case "Delay":
                    NewTaskCommand.CommandType = CommandType.Delay;
                    if (NewTaskDelayVariable == 0)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>("Delay must be greater than 0.", "ShowMessageDialog"));
                        return;
                    }
                    NewTaskCommand.ExtraInfo = NewTaskDelayPeriodVariable;
                    switch (NewTaskDelayPeriodVariable)
                    {
                        case "Second(s)":
                            NewTaskCommand.Variable = NewTaskDelayVariable.ToString();
                            break;
                        case "Minute(s)":
                            NewTaskCommand.Variable = (NewTaskDelayVariable * 60).ToString();
                            break;
                        case "Hour(s)":
                            NewTaskCommand.Variable = (NewTaskDelayVariable * 3600).ToString();
                            break;
                        case "Day(s)":
                            NewTaskCommand.Variable = (NewTaskDelayVariable * 86400).ToString();
                            break;
                        default:
                            return;
                    }
                    break;
                case "Broadcast":
                    if (string.IsNullOrWhiteSpace(NewTaskBroadcastVariable))
                    {
                        Messenger.Default.Send(new NotificationMessage<string>("Please enter the message to broadcast.", "ShowMessageDialog"));
                        return;
                    }
                    NewTaskCommand.CommandType = CommandType.Broadcast;
                    NewTaskCommand.Variable = NewTaskBroadcastVariable;
                    break;
                case "List Players":
                    NewTaskCommand.CommandType = CommandType.ListPlayers;
                    break;
                case "Shutdown The Server":
                    NewTaskCommand.CommandType = CommandType.Shutdown;
                    break;
                case "Save The Current World":
                    NewTaskCommand.CommandType = CommandType.SaveWorld;
                    break;
                case "Custom Command":
                    if (string.IsNullOrWhiteSpace(NewTaskCustomVariable))
                    {
                        Messenger.Default.Send(new NotificationMessage<string>("Please enter custom command text.", "ShowMessageDialog"));
                        return;
                    }
                    NewTaskCommand.CommandType = CommandType.Custom;
                    NewTaskCommand.Variable = NewTaskCustomVariable;
                    break;
                default:
                    return;
            }
            TaskList.Add(NewTaskCommand);
            RaisePropertyChanged("TaskList");
            DontAddTask();
        }

        private void DelTaskCommand()
        {
            if (SelectedTaskCommand == null)
            {
                Messenger.Default.Send(new NotificationMessage<string>("Please select a Task command to remove.", "ShowMessageDialog"));
                return;
            }
            var task = SelectedTaskCommand;
            SelectedTaskCommand = null;
            TaskList.Remove(task);
            RaisePropertyChanged("TaskList");
        }

        private void AddTask()
        {
            NewTaskCommand = new TaskCommand();
            AddTaskCommandWindowVis = true;
        }

        private void ShowAbout()
        {
            Messenger.Default.Send(new NotificationMessage("ShowAboutWindow"));
        }

        private void CancelAddNewTask()
        {
            NewScheduledTask = new ScheduledTask();
            TaskList.Clear();
            RaisePropertyChanged("NewScheduledTask");
            AddTaskWindowVis = false;
        }

        private void ConfirmAddNewTask()
        {
            if (string.IsNullOrWhiteSpace(NewScheduledTask.TaskName))
            {
                Messenger.Default.Send(new NotificationMessage<string>("Please enter a Task Name.", "ShowMessageDialog"));
                return;
            }

            switch (NewScheduledTask.IntervalUnit)
            {
                case "Second(s)":
                    break;
                case "Minute(s)":
                    NewScheduledTask.RepeatInterval = NewScheduledTask.RepeatInterval * 60;
                    break;
                case "Hour(s)":
                    NewScheduledTask.RepeatInterval = NewScheduledTask.RepeatInterval * 3600;
                    break;
                case "Day(s)":
                    NewScheduledTask.RepeatInterval = NewScheduledTask.RepeatInterval * 86400;
                    break;
                default:
                    return;
            }
            NewScheduledTask.TaskCommands = new List<TaskCommand>();
            foreach (TaskCommand command in TaskList)
            {
                NewScheduledTask.TaskCommands.Add(command);
            }
            //Messenger.Default.Send(new NotificationMessage<ScheduledTask>(NewScheduledTask, "AddNewTask"));
            App.Locator.Scheduled.ScheduledTasks.Tasks.Add(NewScheduledTask);
            CancelAddNewTask();
        }

        #region Task Commands Enum
        private IEnumerable<TaskCommands> GetCMEnum()
        {
            return Enum.GetValues(typeof(CommandType)).Cast<Enum>().Select(value => new
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
        }
        #endregion

#endregion

        public override void Cleanup()
        {
            base.Cleanup();
            Messenger.Default.Unregister<NotificationMessage>(this);
        }

        

    }
}
