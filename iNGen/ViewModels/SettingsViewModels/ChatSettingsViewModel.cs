using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;

namespace iNGen.ViewModels.SettingsViewModels
{
    public class ChatSettingsViewModel: ViewModelBase
    {
        public ChatSettings ChatSettings {get; set;}
        public RelayCommand SelectSoundCommand { get; set; }

        public ChatSettingsViewModel()
        {
            ChatSettings = App.ModelManager.Get<UserSettings>().ChatSettings;
            SelectSoundCommand = new RelayCommand(SelectSoundFile);
        }

        private void SelectSoundFile()
        {
            var ofd = new OpenFileDialog { Filter = "Audio files|*.mp3;*.wav" };
            var result = ofd.ShowDialog();
            if (result == false) return;
            ChatSettings.NotificationSoundFile = ofd.FileName;
        }
    }
}
