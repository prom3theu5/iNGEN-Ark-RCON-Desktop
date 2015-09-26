using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
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
        public RelayCommand SendActualPMCommand { get; set; }
        public RelayCommand CancelSendPMCommand { get; set; }
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
            SendActualPMCommand = new RelayCommand(SendActualPM);
            CancelSendPMCommand = new RelayCommand(CancelSendPM);


            #endregion

            PrivateMessage = string.Empty;

        }

        private void CancelSendPM()
        {
            Messenger.Default.Send(new NotificationMessage("PMMessageSent"));
            PrivateMessage = string.Empty;
        }

        private void SendActualPM()
        {
            if (SelectedPlayer == null) return;
            if (string.IsNullOrWhiteSpace(PrivateMessage) || string.IsNullOrEmpty(PrivateMessage)) return;
            App.ArkRcon.SendPrivateMessage(PrivateMessage, SelectedPlayer.SteamID);
            if (App.Locator.ConsoleSettings.ConsoleSettings.IsTimestampingEnabled)
                App.Locator.Console.ConsoleMessages.Add($"{DateTime.Now.ToString("(hh:mm tt)" )} PM Sent to Player: {SelectedPlayer.Name}, with Message: {PrivateMessage}");
            else
                App.Locator.Console.ConsoleMessages.Add($"PM Sent to Player: {SelectedPlayer.Name}, with Message: {PrivateMessage}");
            Messenger.Default.Send(new NotificationMessage("PMMessageSent"));
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
            Messenger.Default.Send(new NotificationMessage("ShowPMWindow"));
        }

        private async Task UpdateSteamPlayerInfo()
        {
            if (string.IsNullOrWhiteSpace(App.ModelManager.Get<UserSettings>().GeneralSettings.SteamApiKey)) return;
            await Task.Run(async () =>
            {
                Client client = new Client(App.ModelManager.Get<UserSettings>().GeneralSettings.SteamApiKey);
                List<string> steamIds = Players.Select(player => player.SteamID.ToString()).ToList();
                await client.GetPlayerBansAsync(steamIds).ContinueWith(async (banData) =>
                {
                    if (banData.Exception == null)
                    {
                        await client.GetPlayerSummariesAsync(steamIds).ContinueWith((players) =>
                        {
                            if (players.Exception == null)
                            {
                                var result = players.Result.Value;
                                foreach (SteamWeb.Models.Player player in result)
                                {
                                    try
                                    {
                                        var gamePlayer = Players.FirstOrDefault(p => p.SteamID.ToString() == player.SteamID.ToString());
                                        var bans = banData.Result.Value.FirstOrDefault(p => p.SteamId.ToString() == player.SteamID.ToString());
                                        if (gamePlayer == null) continue;
                                        gamePlayer.UpdateSteamPlayerData(player);
                                        if (bans != null)
                                            gamePlayer.UpdateSteamBansData(bans);
                                    }
                                    catch { }
                                }
                            }
                        });
                    }
                });
            });
        }
    }
}
