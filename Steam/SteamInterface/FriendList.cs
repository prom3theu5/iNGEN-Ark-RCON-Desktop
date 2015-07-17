using Newtonsoft.Json.Linq;
using SteamWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.SteamInterface
{
    public class FriendListRequest: SteamRequestBase
    {
        public FriendListRequest(string apiKey, string steamid):
            base("ISteamUser", "GetFriendList", 1, apiKey, string.Format("steamid={0}", steamid)){}
    }
    
    public class FriendListResponse: SteamResponseBase<List<Friend>>
    {
        public FriendListResponse(Result<string> steamResponse): base(steamResponse){}
        protected override List<Friend> Parse(string result)
        {
            return JObject.Parse(result)["friendslist"]["friends"].ToObject<List<Friend>>();
        }
    }
}
