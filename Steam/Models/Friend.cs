using Newtonsoft.Json;
using SteamWeb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Friend
    {
        /// <summary>
        /// 64 bit Steam ID of the player.
        /// </summary>
        public ulong PlayerSteamID {get; set;}

        /// <summary>
        /// 64 bit Steam ID of the friend.
        /// </summary>
        [JsonProperty("steamid")]
        public ulong FriendSteamID {get; set;}

        /// <summary>
        /// Relationship qualifier
        /// </summary>
        [JsonProperty("relationship")]
        public string Relationship {get; set;}

        /// <summary>
        /// Unix timestamp of the time when the relationship was created.
        /// </summary>
        [JsonProperty("friend_since")]
        public int FriendSince {get; set;}

        public override bool Equals(object obj)
        {
            var other = obj as Friend;
            return other != null && PlayerSteamID.Equals(other.PlayerSteamID) && FriendSteamID.Equals(other.FriendSteamID);
        }

        public override int GetHashCode()
        {
            return new HashCodeBuilder(PlayerSteamID).Add(FriendSteamID).GetHashCode();
        }
    }
}
