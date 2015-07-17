using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Player
    {
        /// <summary>
        /// 64bit SteamID of the user.
        /// Visibility: Public
        /// </summary>
        [JsonProperty("steamid")]
        public ulong SteamID {get; set;}
        
        /// <summary>
        /// The player's persona name (display name).
        /// Visibility: Public
        /// </summary>
        [JsonProperty("personaname")]
        public string PersonaName {get; set;}
        
        /// <summary>
        /// The full URL of the player's Steam Community profile.
        /// Visibility: Public
        /// </summary>
        [JsonProperty("profileurl")]
        public string ProfileURL {get; set;}
        
        /// <summary>
        /// The full URL of the player's 32x32px avatar. If the user hasn't configured an avatar, this will be the default ? avatar.
        /// Visibility: Public
        /// </summary>
        [JsonProperty("avatar")]
        public string AvatarSmallURL {get; set;}
        
        /// <summary>
        /// The full URL of the player's 64x64px avatar. If the user hasn't configured an avatar, this will be the default ? avatar.
        /// Visibility: Public
        /// </summary>
        [JsonProperty("avatarmedium")]
        public string AvatarMediumURL {get; set;}
        
        /// <summary>
        /// The full URL of the player's 184x184px avatar. If the user hasn't configured an avatar, this will be the default ? avatar.
        /// Visibility: Public
        /// </summary>
        [JsonProperty("avatarfull")]
        public string AvatarFullURL {get; set;}
        
        /// <summary>
        /// The user's current status. 0 - Offline, 1 - Online, 2 - Busy, 3 - Away, 4 - Snooze, 5 - looking to trade, 6 - looking to play.
        /// If the player's profile is private, this will always be "0", except is the user has set his status to looking to trade or looking
        /// to play, because a bug makes those status appear even if the profile is private.
        /// Visibility: Public
        /// </summary>
        [JsonProperty("personastate")]
        public PersonaState PersonaState {get; set;}
        
        /// <summary>
        /// This represents whether the profile is visible or not, and if it is visible, why you are allowed to see it. Note that because
        /// this WebAPI does not use authentication, there are only two possible values returned: 1 - the profile is not visible to
        /// you (Private, Friends Only, etc), 3 - the profile is "Public", and the data is visible. Mike Blaszczak's post on Steam forums
        /// says, "The community visibility state this API returns is different than the privacy state. It's the effective visibility state
        /// from the account making the request to the account being viewed given the requesting account's relationship to the viewed account."
        /// Visibility: Public
        /// </summary>
        [JsonProperty("communityvisibilitystate")]
        public CommunityVisibilityState CommunityVisibilityState {get; set;}
        
        /// <summary>
        /// If set, indicates the user has a community profile configured (will be set to '1').
        /// Visibility: Public
        /// </summary>
        [JsonProperty("profilestate")]
        public ProfileState ProfileState {get; set;}
        
        /// <summary>
        /// The last time the user was online, in unix time.
        /// Visibility: Public
        /// </summary>
        [JsonProperty("lastlogoff")]
        public int LastLogOff {get; set;}
        
        /// <summary>
        /// If set, indicates the profile allows public comments.
        /// Visibility: Public
        /// </summary>
        [JsonProperty("commentpermission")]
        public int CommentPermission {get; set;}
        
        /// <summary>
        /// The player's "Real Name", if they have set it.
        /// Visibility: Private
        /// </summary>
        [JsonProperty("realname")]
        public string RealName {get; set;}
        
        /// <summary>
        /// The player's primary group, as configured in their Steam Community profile.
        /// Visibility: Private
        /// </summary>
        [JsonProperty("primaryclanid")]
        public long PrimaryClanID {get; set;}
        
        /// <summary>
        /// The time the player's account was created.
        /// Visibility: Private
        /// </summary>
        [JsonProperty("timecreated")]
        public int TimeCreated {get; set;}
        
        /// <summary>
        /// If the user is currently in-game, this value will be returned and set to the gameid of that game.
        /// Visibility: Private
        /// </summary>
        [JsonProperty("gameid")]
        public long GameID {get; set;}
        
        /// <summary>
        /// The ip and port of the game server the user is currently playing on, if they are playing on-line in a game using Steam
        /// matchmaking. Otherwise will be set to "0.0.0.0:0".
        /// Visibility: Private
        /// </summary>
        [JsonProperty("gameserverip")]
        public string GameServerIP {get; set;}
        
        /// <summary>
        /// If the user is currently in-game, this will be the name of the game they are playing. This may be the name of a non-Steam game shortcut.
        /// Visibility: Private
        /// </summary>
        [JsonProperty("gameextrainfo")]
        public string GameExtraInfo {get; set;}
        
        /// <summary>
        /// If set on the user's Steam Community profile, The user's country of residence, 2-character ISO country code.
        /// Visibility: Private
        /// </summary>
        [JsonProperty("loccountrycode")]
        public string LocationCountryCode {get; set;}
        
        /// <summary>
        /// If set on the user's Steam Community profile, The user's state of residence.
        /// Visibility: Private
        /// </summary>
        [JsonProperty("locstatecode")]
        public string LocationStateCode {get; set;}
        
        /// <summary>
        /// An internal code indicating the user's city of residence. A future update will provide this data in a more useful way.
        /// Visibility: Private
        /// </summary>
        [JsonProperty("loccityid")]
        public int LocationCityID {get; set;}

        public override bool Equals(object obj)
        {
            var other = obj as Player;
            return other != null && SteamID.Equals(other.SteamID);
        }

        public override int GetHashCode()
        {
            return SteamID.GetHashCode();
        }
    }
}