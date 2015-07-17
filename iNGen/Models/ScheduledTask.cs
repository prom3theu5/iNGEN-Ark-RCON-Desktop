using PropertyChanged;
using ProtoBuf;
using PTK.ModelManager;
using PTK.Utils;
using PTK.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iNGen.Models
{
    public class TaskCommands
    {
        public string Description { get; set; }
        public Enum value { get; set; }
    }
    
    public enum CommandType
    {
        [Description("Delay")]
        Delay,
        [Description("Broadcast")]
        Broadcast,
        [Description("List Players")]
        ListPlayers,
        [Description("Shutdown The Server")]
        Shutdown,
        [Description("Save The Current World")]
        SaveWorld,
        [Description("Custom Command")]
        Custom
    }

    [Model("SavedTasks")]
    [ProtoContract]
    public class ScheduledTaskSet : Notifiable
    {
        [ProtoMember(1)]
        public ObservableCollection<ScheduledTask> Tasks { get; set; }

        public ScheduledTaskSet()
        {
            Tasks = new ObservableCollection<ScheduledTask>();
            Tasks.CollectionChanged += Tasks_CollectionChanged;
        }

        void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var task = item as ScheduledTask;
                    Helpers.TaskCreationDestruction.CreateTask(task);
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var task = item as ScheduledTask;
                    Helpers.TaskCreationDestruction.CancelTask(task);
                }
            }

            OnPropertyChanged("ScheduledTasks");
        }
    }

    [ImplementPropertyChanged]
    [ProtoContract]
    public class ScheduledTask
    {
       [ProtoMember(1)]
       public string TaskName { get; set; }
       [ProtoMember(2)]
       public List<TaskCommand> TaskCommands { get; set; }
       [ProtoMember(3)]
       public bool IsRepeat { get; set; }
       [ProtoMember(4)]
       public int RepeatInterval { get; set; }
       [ProtoMember(5)]
       public bool IsEnabled { get; set; }
       [ProtoMember(6)]
       public string IntervalUnit { get; set; }
       public Task ActualTask { get; set; }
       public CancellationTokenSource TaskCancellationTokenSource { get; set; }
       public int IntervalAsUnit
       {
           get
           {
               switch (IntervalUnit)
               {
                   case "Second(s)":
                       return RepeatInterval;
                   case "Minute(s)":
                       return RepeatInterval/60;
                   case "Hour(s)":
                       return RepeatInterval/3600;
                   case "Day(s)":
                       return RepeatInterval/86400;
                   default:
                       return 0;
               }
           }
       }
   }

    [ProtoContract]
    [ImplementPropertyChanged]
    public class TaskCommand
    {
       [ProtoMember(1)]
        public CommandType CommandType { get; set; }
        [ProtoMember(2)]
        public string Variable { get; set; }
        public string ExtraInfo { get; set; }
    }
}
