using PTK.WPF;
using iNGen.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace iNGen.ViewModels
{
    public class NavigationViewModel: Notifiable
    {
        public ObservableCollection<NavigationItem> NavigationItems {get; set;}

        public NavigationViewModel()
        {
            NavigationItems = new ObservableCollection<NavigationItem>();

            AddNavigationItem("Home", new HomeView());
            AddNavigationItem("Players", new PlayersView());
            AddNavigationItem("Console", new ConsoleView());
            AddNavigationItem("Chat", new ChatView());
            AddNavigationItem("Scheduled Tasks", new ScheduledCommandsView());
            AddNavigationItem("iNGen Settings", new iNGenSettingsView());
        }

        private void AddNavigationItem(string header, UserControl content)
        {
            NavigationItems.Add(new NavigationItem(){Header = header, Content = content});
        }
    }

    public class NavigationItem
    {
        public string Header {get; set;}
        public UserControl Content {get; set;}
    }
}
