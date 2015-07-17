using PTK.WPF;
using SteamWeb;
using System;
using System.Collections.Generic;
using System.Windows;

namespace iNGen.Models
{
    public class Player : Notifiable
    {
        private ulong mSteamID;
        private string mName;
        private int mCurrentSessionConnectionDuration;
        private bool mIsConnected;
        private bool mIsOnline;

        public ulong SteamID
        {
            get { return mSteamID; }
            set { SetField(ref mSteamID, value); }
        }

        public string Name
        {
            get { return mName; }
            set { SetField(ref mName, value); }
        }

        public int CurrentSessionConnectionDuration
        {
            get { return mCurrentSessionConnectionDuration; }
            set { SetField(ref mCurrentSessionConnectionDuration, value); }
        }

        public bool IsConnected
        {
            get { return mIsConnected; }
            set { SetField(ref mIsConnected, value); }
        }

        public bool IsOnline
        {
            get { return mIsOnline; }
            set { mIsOnline = value; }
        }
        
        public bool IsSteamProfilePrivate { get; set; }
        public bool IsSteamProfileConfigured { get; set; }
        public int SteamProfileTimeCreated { get; set; }
        public bool IsFamilyShareAccount { get; set; }
        public bool IsNotPrivateAndHasName { get
            {
                return !IsSteamProfilePrivate && !string.IsNullOrWhiteSpace(RealName);
            }
        }
        public ulong FamilyShareLenderSteamID { get; set; }
        public string RealName { get; set; }

        public string CountryCode { get; set; }
        public object CountryImage
        {
            get
            {
                if (IsSteamProfilePrivate || string.IsNullOrWhiteSpace(CountryCode)) return DependencyProperty.UnsetValue;
                return string.Format("/iNGen;component/Resources/Countries/{0}.png", CountryCode.ToLower());
            }
        }

        public HashSet<ulong> KnownFriends { get; set; }

        public int SteamGameCount { get; set; }
        public int ArkPlaytime { get; set; }
        public bool HasVACBan { get; set; }
        public int VACBanCount { get; set; }
        public int DaysSinceLastVACBan { get; set; }
        public string LastBanDate
        {
            get
            {
                if (!HasVACBan) return null;
                return DateTime.Today.AddDays((DaysSinceLastVACBan * -1)).ToLongDateString();
            }
        }

        public bool HasServerBan { get; set; }
        public string ServerBanReason { get; set; }

        public string AvatarSmallURL { get; set; }
        public string AvatarMediumURL { get; set; }
        public string AvatarFullURL { get; set; }
        public string ProfileURL
        {
            get
            {
                return @"http://steamcommunity.com/id/" + SteamID.ToString();
            }
        }


        public Player(Ark.Models.Player player)
        {
            Name = player.Name;
            SteamID = player.SteamID;
        }

        public void UpdateSteamPlayerData(SteamWeb.Models.Player steamPlayer)
        {
            Name = steamPlayer.PersonaName;

            AvatarSmallURL = steamPlayer.AvatarSmallURL;
            AvatarMediumURL = steamPlayer.AvatarMediumURL;
            AvatarFullURL = steamPlayer.AvatarFullURL;

            IsSteamProfilePrivate = steamPlayer.CommunityVisibilityState != SteamWeb.CommunityVisibilityState.Public;
            IsSteamProfileConfigured = steamPlayer.ProfileState == SteamWeb.ProfileState.Setup;
            SteamProfileTimeCreated = steamPlayer.TimeCreated;

            if (!IsSteamProfilePrivate)
            {
                RealName = steamPlayer.RealName;
                CountryCode = steamPlayer.LocationCountryCode;
            }
        }

        public void UpdateSteamBansData(SteamWeb.Models.PlayerBans bans)
        {
            HasVACBan = bans.IsVACBanned;
            VACBanCount = bans.VACBanCount;
            DaysSinceLastVACBan = bans.DaysSinceLastBan;
        }

        public void UpdateSteamGamesData(SteamWeb.Models.OwnedGames games)
        {
            SteamGameCount = games.GameCount;
        }

        public void UpdateSteamFriendData(SteamWeb.Models.Friend friend)
        {
            KnownFriends.Add(friend.FriendSteamID);
        }

        public void UpdateFamilyShareData(SteamWeb.Models.FamilyShareAccount familyShare)
        {
            IsFamilyShareAccount = familyShare.IsFamilyShareAccount;
            FamilyShareLenderSteamID = familyShare.LenderSteamID;
        }
        
    }
}
