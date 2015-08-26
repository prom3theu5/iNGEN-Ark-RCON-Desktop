using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace iNGen.ViewModels.SettingsViewModels
{
    public class ChatSettingsViewModel: ViewModelBase
    {
        public ChatSettings ChatSettings {get; set;}

        public ChatSettingsViewModel()
        {
            ChatSettings = App.ModelManager.Get<UserSettings>().ChatSettings;
        }
    }
}
