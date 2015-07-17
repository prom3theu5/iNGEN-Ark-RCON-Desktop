using Newtonsoft.Json.Linq;
using SteamWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.SteamInterface
{
    public class PlayerBansRequest: SteamRequestBase
    {
        public PlayerBansRequest(string apiKey, params long[] steamid):
            this(apiKey, steamid.Select(s => s.ToString()).ToArray()){}

        public PlayerBansRequest(string apiKey, params string[] steamid):
            base("ISteamUser", "GetPlayerBans", 1, apiKey, string.Format("steamids=[{0}]", string.Join(",", steamid))){}
    }

    public class PlayerBansResponse: SteamResponseBase<List<PlayerBans>>
    {
        public PlayerBansResponse(Result<string> steamResponse): base(steamResponse){}
        protected override List<PlayerBans> Parse(string result)
        {
            return JObject.Parse(result)["players"].ToObject<List<PlayerBans>>();
        }
    }
}
