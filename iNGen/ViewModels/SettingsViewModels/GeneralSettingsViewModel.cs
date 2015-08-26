using iNGen.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Diagnostics;

namespace iNGen.ViewModels.SettingsViewModels
{
    public class GeneralSettingsViewModel: ViewModelBase
    {
        public GeneralSettings GeneralSettings {get; set;}
        public RelayCommand GetSteamKeyCommand { get; set; }
        
        public GeneralSettingsViewModel()
        {
            GeneralSettings = App.ModelManager.Get<UserSettings>().GeneralSettings;
            GetSteamKeyCommand = new RelayCommand(GetSteamKey);
        }

        private void GetSteamKey()
        {
            Process.Start("http://steamcommunity.com/dev/apikey");
        }
    }
}
