using PTK.WPF;
using iNGen.Views.SettingsViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace iNGen.ViewModels
{
    public class SettingsSection
    {
        public string Title {get; set;}
        public UserControl Content {get; set;}
    }
    public class iNGenSettingsViewModel: Notifiable
    {
        public ObservableCollection<SettingsSection> Sections {get; set;}

        public iNGenSettingsViewModel()
        {
            Sections = new ObservableCollection<SettingsSection>();

            Sections.Add(new SettingsSection(){Title = "General Settings", Content = new GeneralSettingsView()});
            Sections.Add(new SettingsSection(){Title = "Console Settings", Content = new ConsoleSettingsView()});
            Sections.Add(new SettingsSection() { Title = "Chat Settings", Content = new ChatSettingsView() });
        }
    }
}
