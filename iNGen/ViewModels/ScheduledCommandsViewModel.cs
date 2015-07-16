using iNGen.Models;
using PTK.WPF;
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
    public class ScheduledCommandsViewModel : Notifiable
    {
        public ScheduledCommandsViewModel()
        {
            ScheduledTasks = new ScheduledTaskSet();
            LoadTasks();
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

        private ScheduledTaskSet _scheduledTasks;

        public ScheduledTaskSet ScheduledTasks
        {
            get { return _scheduledTasks; }
            set { SetField(ref _scheduledTasks, value); }
        }

    }
}
