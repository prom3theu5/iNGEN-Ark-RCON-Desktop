using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using iNGen.Models;
using PTK.WPF;
using SteamWeb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace iNGen.ViewModels
{
    public class PlayersViewModel : ViewModelBase
    {
        private const string SteamAPIKey = "5AA89FF9AF1C9DEADC75EF27339A6760";

        public ObservableCollection<Player> Players { get; set; }
        public Player SelectedPlayer { get; set; }
        public RelayCommand OpenSteamProfileCommand { get; set; }
        public RelayCommand CopySteamIDCommand { get; set; }
        public RelayCommand CopyPlayerNameCommand { get; set; }
        public RelayCommand KickSelectedCommand { get; set; }
        public RelayCommand BanSelectedCommand { get; set; }
        public RelayCommand WhitelistSelectedCommand { get; set; }
        public RelayCommand UnWhitelistSelectedCommand { get; set; }
        public RelayCommand PMSelectedPlayerCommand { get; set; }
        public RelayCommand RefreshPlayersCommand { get; set; }
        public string PrivateMessage { get; set; }

        public PlayersViewModel()
        {
            Players = new ObservableCollection<Player>();

            #region RCON Events
            App.ArkRcon.PlayersUpdated += async (s, args) =>
            {
                Players.Clear();
                foreach (Ark.Models.Player player in args.Players)
                {
                    Players.Add(new Models.Player(player));
                }
                await UpdateSteamPlayerInfo();
            };
            #endregion
            
            #region Buttons and Events

            OpenSteamProfileCommand = new RelayCommand(OpenSteamProfile);
            CopySteamIDCommand = new RelayCommand(CopySteamID);
            CopyPlayerNameCommand = new RelayCommand(CopyPlayerName);
            KickSelectedCommand = new RelayCommand(KickSelectedPlayer);
            BanSelectedCommand = new RelayCommand(BanSelectedPlayer);
            WhitelistSelectedCommand = new RelayCommand(WhitelistSelectedPlayer);
            UnWhitelistSelectedCommand = new RelayCommand(UnWhitelistSelectedPlayer);
            PMSelectedPlayerCommand = new RelayCommand(PMSelectedPlayer);
            RefreshPlayersCommand = new RelayCommand(RefreshPlayers);
            
            #endregion

            PrivateMessage = string.Empty;

        }

        private void RefreshPlayers()
        {
            App.ArkRcon.ExecCommand(Ark.Opcode.GetPlayers, "listplayers");
        }

        private void OpenSteamProfile()
        {
            if (SelectedPlayer == null) return;
            Process.Start("http://steamcommunity.com/profiles/" + SelectedPlayer.SteamID);
        }

        private void CopySteamID()
        {
            if (SelectedPlayer == null) return;
            Clipboard.SetText(SelectedPlayer.SteamID.ToString());
        }

        private void CopyPlayerName()
        {
            if (SelectedPlayer == null) return;
            Clipboard.SetText(SelectedPlayer.Name);
        }

        private void KickSelectedPlayer()
        {
            if (SelectedPlayer == null) return;
            App.ArkRcon.KickPlayer(SelectedPlayer.SteamID);
        }

        private void BanSelectedPlayer()
        {
            if (SelectedPlayer == null) return;
            App.ArkRcon.BanPlayer(SelectedPlayer.SteamID);
        }

        private void WhitelistSelectedPlayer()
        {
            if (SelectedPlayer == null) return;
            App.ArkRcon.WhitelistPlayer(SelectedPlayer.SteamID);
        }

        private void UnWhitelistSelectedPlayer()
        {
            if (SelectedPlayer == null) return;
            App.ArkRcon.UnWhitelistPlayer(SelectedPlayer.SteamID);
        }

        private void PMSelectedPlayer()
        {
            if (SelectedPlayer == null) return;
            
        }

        private async Task UpdateSteamPlayerInfo()
        {
            await Task.Run(async () =>
            {
                Client client = new Client(SteamAPIKey);
                List<string> steamIds = Players.Select(player => player.SteamID.ToString()).ToList();
                var banData = await client.GetPlayerBansAsync(steamIds);
                await client.GetPlayerSummariesAsync(steamIds).ContinueWith((players) =>
                {
                    if (players.Exception == null)
                    {
                        var result = players.Result.Value;
                        foreach (SteamWeb.Models.Player player in result)
                        {
                            var gamePlayer = Players.FirstOrDefault(p => p.SteamID.ToString() == player.SteamID.ToString());
                            var bans = banData.Value.FirstOrDefault(p => p.SteamId.ToString() == player.SteamID.ToString());
                            if (gamePlayer == null) continue;
                            gamePlayer.UpdateSteamPlayerData(player);
                            if (bans != null)
                                gamePlayer.UpdateSteamBansData(bans);
                        }
                    }
                });

            });
        }
    }
}
