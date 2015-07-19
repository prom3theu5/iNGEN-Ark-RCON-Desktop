using iNGen.Models;
using PTK.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iNGen.Helpers
{
   public static class TaskCreationDestruction
   {
       public static void CancelTask(ScheduledTask task)
       {
           if (task.TaskCancellationTokenSource != null)
           {
               task.TaskCancellationTokenSource.Cancel();
           }
       }

       public static void CreateTask(ScheduledTask task)
       {
           if (task.IsEnabled)
           {
               task.TaskCancellationTokenSource = new CancellationTokenSource();
               task.ActualTask = Repeat.Interval(TimeSpan.FromSeconds(task.RepeatInterval), new Action(async () =>
               {
                   await App.Current.Dispatcher.BeginInvoke(new Action(async () =>
                   {
                       if (App.ArkRcon.IsConnected)
                       {
                           foreach (TaskCommand command in task.TaskCommands)
                           {
                               switch (command.CommandType)
                               {
                                   case CommandType.Delay:
                                       bool result;
                                       int output;
                                       result = int.TryParse(command.Variable, out output);
                                       await Task.Delay(output * 1000);
                                       break;
                                   case CommandType.ListPlayers:
                                       App.ArkRcon.ExecuteScheduledTask(task.TaskName, "listplayers");
                                       break;
                                   case CommandType.Broadcast:
                                       App.ArkRcon.ExecuteScheduledTask(task.TaskName, string.Format("broadcast {0}", command.Variable));
                                       break;
                                   case CommandType.Shutdown:
                                       App.ArkRcon.ExecuteScheduledTask(task.TaskName, "DoExit");
                                       break;
                                   case CommandType.SaveWorld:
                                       App.ArkRcon.ExecuteScheduledTask(task.TaskName, "SaveWorld");
                                       break;
                                   case CommandType.Custom:
                                       App.ArkRcon.ExecuteScheduledTask(task.TaskName, command.Variable);
                                       break;
                                   default:
                                       break;
                               }
                           }

                       }
                   }));
               }), task.TaskCancellationTokenSource.Token, task.IsRepeat);
           }
       } 
   }
}
