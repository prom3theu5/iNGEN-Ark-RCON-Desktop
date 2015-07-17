using Newtonsoft.Json.Linq;
using SteamWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.SteamInterface
{
    public class OwnedGamesRequest: SteamRequestBase
    {
        public OwnedGamesRequest(string apiKey, string steamid, bool detailedInfo = false, bool includeFreeGames = true, params uint[] filter):
            base("IPlayerService", "GetOwnedGames", 1, apiKey,
            String.Format("steamid={0}", steamid),
            String.Format("include_appinfo={0}", detailedInfo ? "1" : "0"),
            String.Format("include_played_free_games={0}", includeFreeGames ? "1" : "0"),
            filter.Length != 0 ? String.Format("appids_filter=[{0}]", string.Join(",", filter)) : null){}
    }

    public class OwnedGamesResponse: SteamResponseBase<OwnedGames>
    {
        public OwnedGamesResponse(Result<string> steamResponse): base(steamResponse){}
        protected override OwnedGames Parse(string result)
        {
            return JObject.Parse(result)["response"].ToObject<OwnedGames>();
        }
    }
}
