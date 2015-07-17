using Ark.Models;
using iNGen.ViewModels;
using SteamWeb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iNGen.Views
{
    public partial class PlayersView : UserControl
    {
        public PlayersViewModel ViewModel { get; set; }

        public PlayersView()
        {
            ViewModel = new PlayersViewModel();
            InitializeComponent();
            
            
            App.ArkRcon.PlayersUpdated += async (s, args) =>
            {
                PlayersListView.ItemsSource = null;
                ViewModel.Players.Clear();
                foreach (Player player in args.Players)
                {
                    ViewModel.Players.Add(new Models.Player(player));
                }
                PlayersListView.ItemsSource = ViewModel.Players;
                await UpdateSteamPlayerInfo();
            };
        }

        private async Task UpdateSteamPlayerInfo()
        {
            await Task.Run(async () =>
            {
                Client client = new Client("5AA89FF9AF1C9DEADC75EF27339A6760");
                List<string> steamIds = ViewModel.Players.Select(player => player.SteamID.ToString()).ToList();
                var banData = await client.GetPlayerBansAsync(steamIds);
                await client.GetPlayerSummariesAsync(steamIds).ContinueWith((players) => {
                    if (players.Exception == null)
                    {
                        var result = players.Result.Value;
                        foreach (SteamWeb.Models.Player player in result)
                        {
                            var gamePlayer = ViewModel.Players.FirstOrDefault(p => p.SteamID.ToString() == player.SteamID.ToString());
                            var bans = banData.Value.FirstOrDefault(p=>p.SteamId.ToString() == player.SteamID.ToString());
                            if (gamePlayer == null) continue;
                                gamePlayer.UpdateSteamPlayerData(player);
                            if (bans != null)
                                gamePlayer.UpdateSteamBansData(bans);
                        }
                        ViewModel.CollectionChanged();
                    }
                });
                
            });
        }

        private void OpenSteamProfileButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayersListView.SelectedItem == null) return;
            var player = PlayersListView.SelectedItem as Models.Player;
            Process.Start("http://steamcommunity.com/profiles/" + player.SteamID);
        }

        private void CopyPlayerSteamIDButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayersListView.SelectedItem == null) return;
            var player = PlayersListView.SelectedItem as Models.Player;
            Clipboard.SetText(player.SteamID.ToString());
        }

        private void CopyPlayerNameButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayersListView.SelectedItem == null) return;
            var player = PlayersListView.SelectedItem as Models.Player;
            Clipboard.SetText(player.Name);
        }

        private void KickButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayersListView.SelectedItem == null) return;
            var player = PlayersListView.SelectedItem as Models.Player;
            App.ArkRcon.KickPlayer(player.SteamID);
        }

        private void BanButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayersListView.SelectedItem == null) return;
            var player = PlayersListView.SelectedItem as Models.Player;
            App.ArkRcon.BanPlayer(player.SteamID);
        }


        private void RefreshPlayersButton_Click(object sender, RoutedEventArgs e)
        {
            App.ArkRcon.ExecCommand(Ark.Opcode.GetPlayers, "listplayers");
        }

        private void PlayerKickButton_Click(object sender, RoutedEventArgs e)
        {
            KickButtonClick(sender, e);
        }

        private void BanPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            BanButtonClick(sender, e);
        }

        private void PrivateMessageClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
