using Newtonsoft.Json.Linq;
using SteamWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.SteamInterface
{
    public class PlayerSummariesRequest: SteamRequestBase
    {
        public PlayerSummariesRequest(string apiKey, IEnumerable<long> steamids):
            this(apiKey, steamids.ToArray()){}

        public PlayerSummariesRequest(string apiKey, params long[] steamids):
            this(apiKey, steamids.Select(s => s.ToString()).ToArray()){}

        public PlayerSummariesRequest(string apiKey, params string[] steamids):
            base("ISteamUser", "GetPlayerSummaries", 2, apiKey, string.Format("steamids=[{0}]", string.Join(",", steamids))){}
    }

    public class PlayerSummariesResponse: SteamResponseBase<List<Player>>
    {
        public PlayerSummariesResponse(Result<string> steamResponse): base(steamResponse){}
        protected override List<Player> Parse(string result)
        {
            return JObject.Parse(result)["response"]["players"].ToObject<List<Player>>();
        }
    }
}
