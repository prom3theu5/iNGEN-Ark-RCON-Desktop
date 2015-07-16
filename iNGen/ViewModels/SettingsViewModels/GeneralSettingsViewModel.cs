using PTK.WPF;
using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels.SettingsViewModels
{
    public class GeneralSettingsViewModel: Notifiable
    {
        public GeneralSettings GeneralSettings {get; set;}

        public GeneralSettingsViewModel()
        {
            GeneralSettings = App.ModelManager.Get<UserSettings>().GeneralSettings;
        }
    }
}
