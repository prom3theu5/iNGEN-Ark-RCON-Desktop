using iNGen.Models;
using PTK.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels
{
     public class PlayersViewModel: Notifiable
    {
        private ObservableCollection<Player> mPlayers;
        public ObservableCollection<Player> Players
        {
            get { return mPlayers; }
            set { SetField(ref mPlayers, value); }
        }

        public PlayersViewModel()
        {
            Players = new ObservableCollection<Player>();       
        }

        void Players_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged();
        }

        public void CollectionChanged()
        {
            OnPropertyChanged("Players");
        }
    }
}
