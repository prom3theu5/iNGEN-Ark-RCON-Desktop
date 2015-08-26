using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace iNGen.ViewModels.SettingsViewModels
{
    public class ConsoleSettingsViewModel: ViewModelBase
    {
        public ConsoleSettings ConsoleSettings {get; set;}

        public ConsoleSettingsViewModel()
        {
            ConsoleSettings = App.ModelManager.Get<UserSettings>().ConsoleSettings;
        }
    }
}
