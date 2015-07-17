using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FamilyShareAccount
    {
        /// <summary>
        /// 64bit SteamID of the user.
        /// </summary>
        [JsonProperty("SteamID")]
        public ulong SteamID {get; set;}

        /// <summary>
        /// 64bit SteamID of the parent family share account.
        /// </summary>
        [JsonProperty("lender_steamid")]
        public ulong LenderSteamID {get; set;}

        public bool IsFamilyShareAccount
        {
            get
            {
                return SteamID != 0;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as FamilyShareAccount;
            return other != null && SteamID.Equals(other.SteamID);
        }

        public override int GetHashCode()
        {
            return SteamID.GetHashCode();
        }
    }
}
