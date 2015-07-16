using PTK.WPF;
using iNGen.Models;
using Ark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels
{
    public class ConsoleViewModel: Notifiable
    {
        public ConsoleSettings ConsoleSettings {get; set;}

        public ConsoleViewModel()
        {
            ConsoleSettings = App.ModelManager.Get<UserSettings>().ConsoleSettings;
        }
    }
}