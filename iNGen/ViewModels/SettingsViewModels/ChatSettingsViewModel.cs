using PTK.WPF;
using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels.SettingsViewModels
{
    public class ChatSettingsViewModel: Notifiable
    {
        public ChatSettings ChatSettings {get; set;}

        public ChatSettingsViewModel()
        {
            ChatSettings = App.ModelManager.Get<UserSettings>().ChatSettings;
        }
    }
}
