using Newtonsoft.Json.Linq;
using SteamWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.SteamInterface
{
    public class FamilyShareAccountRequest: SteamRequestBase
    {
        public FamilyShareAccountRequest(string apiKey, string steamid, int appID):
            base("IPlayerService", "IsPlayingSharedGame", 1, apiKey,
            String.Format("steamid={0}", steamid),
            String.Format("appid_playing={0}", appID.ToString())){}
    }

    public class FamilyShareAccountResponse: SteamResponseBase<FamilyShareAccount>
    {
        public FamilyShareAccountResponse(Result<string> steamResponse): base(steamResponse){}
        protected override FamilyShareAccount Parse(string result)
        {
            return JObject.Parse(result)["response"].ToObject<FamilyShareAccount>();
        }
    }
}
