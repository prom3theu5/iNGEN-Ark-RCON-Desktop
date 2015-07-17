using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamWeb.Models;
using SteamWeb.SteamInterface;
using SteamWeb.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb
{
    public class Client: IDisposable
    {
        public readonly string APIKey;
        private HttpClient HttpClient;

        public Client(string apikey)
        {
            APIKey = apikey;
            HttpClient = new HttpClient();
        }

        private async Task<Result<string>> SendRequestAsync(SteamRequestBase request)
        {
            Debug.WriteLine("Requesting: " + request.Request);
            HttpResponseMessage response = await HttpClient.GetAsync(request.Request);

            if(response.IsSuccessStatusCode)
                return Result<string>.Ok(await response.Content.ReadAsStringAsync());
            else
                return Result<string>.Err(response.ReasonPhrase);
        }
        
        
        ////////////////////////////////////////////////////////////////////////////////
        /// Player's Friend List
        ////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns the friend list of any Steam user, provided his Steam Community profile visibility is set to "Public".
        /// </summary>
        public async Task<Result<List<Friend>>> GetFriendListAsync(string steamid)
        {
            return new FriendListResponse(await SendRequestAsync(new FriendListRequest(APIKey, steamid))).Result;
        }

        
        ////////////////////////////////////////////////////////////////////////////////
        /// Player's Bans
        ////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns Community, VAC, and Economy ban statuses for given players.
        /// </summary>
        public async Task<Result<List<PlayerBans>>> GetPlayerBansAsync(IEnumerable<string> steamids)
        {
            var playerbans = new List<PlayerBans>();
            var splitSteamids = steamids.Split(MaxSteamBatchRequestCount);

            foreach(var steamidGroup in splitSteamids)
            {
                var response = new PlayerBansResponse(await SendRequestAsync(new PlayerBansRequest(APIKey, steamidGroup)));

                if(response.Result.IsOk)
                    playerbans.AddRange(response.Result.Value);
                else
                    return Result<List<PlayerBans>>.Err(response.Result.ErrorMessage);
            }

            return Result<List<PlayerBans>>.Ok(playerbans);
        }
        
        /// <summary>
        /// Returns Community, VAC, and Economy ban statuses for given players.
        /// </summary>
        public async Task<Result<List<PlayerBans>>> GetPlayerBansAsync(params long[] steamids)
        {
            return await GetPlayerBansAsync(steamids.Select(id => id.ToString()));
        }
        
        /// <summary>
        /// Returns Community, VAC, and Economy ban statuses for given players.
        /// </summary>
        public async Task<Result<List<PlayerBans>>> GetPlayerBansAsync(params string[] steamids)
        {
            return await GetPlayerBansAsync(steamids);
        }

        ////////////////////////////////////////////////////////////////////////////////
        /// Player Summaries
        ////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns basic profile information for a list of 64-bit Steam IDs.
        /// Some data associated with a Steam account may be hidden if the user
        /// has their profile visibility set to "Friends Only" or "Private".
        /// In that case, only public data will be returned.
        /// </summary>
        public async Task<Result<List<Player>>> GetPlayerSummariesAsync(IEnumerable<string> steamids)
        {
            var players = new List<Player>();
            var splitSteamids = steamids.Split(MaxSteamBatchRequestCount);
            
            foreach(var steamidGroup in splitSteamids)
            {
                var response = new PlayerSummariesResponse(await SendRequestAsync(new PlayerSummariesRequest(APIKey, steamidGroup)));
                
                if(response.Result.IsOk)
                    players.AddRange(response.Result.Value);
                else
                    return Result<List<Player>>.Err(response.Result.ErrorMessage);
            }

            return Result<List<Player>>.Ok(players);
        }

        /// <summary>
        /// Returns basic profile information for a list of 64-bit Steam IDs.
        /// Some data associated with a Steam account may be hidden if the user
        /// has their profile visibility set to "Friends Only" or "Private".
        /// In that case, only public data will be returned.
        /// </summary>
        public async Task<Result<List<Player>>> GetPlayerSummariesAsync(IEnumerable<long> steamids)
        {
            return await GetPlayerSummariesAsync(steamids.Select(id => id.ToString()));
        }

        /// <summary>
        /// Returns basic profile information for a list of 64-bit Steam IDs.
        /// Some data associated with a Steam account may be hidden if the user
        /// has their profile visibility set to "Friends Only" or "Private".
        /// In that case, only public data will be returned.
        /// </summary>
        public async Task<Result<List<Player>>> GetPlayerSummariesAsync(params long[] steamids)
        {
            return await GetPlayerSummariesAsync(steamids.Select(id => id.ToString()));
        }

        /// <summary>
        /// Returns basic profile information for a list of 64-bit Steam IDs.
        /// Some data associated with a Steam account may be hidden if the user
        /// has their profile visibility set to "Friends Only" or "Private".
        /// In that case, only public data will be returned.
        /// </summary>
        public async Task<Result<List<Player>>> GetPlayerSummariesAsync(params string[] steamids)
        {
            return await GetPlayerSummariesAsync(steamids);
        }

        ////////////////////////////////////////////////////////////////////////////////
        /// Player's Owned Games
        ////////////////////////////////////////////////////////////////////////////////
       
        /// <summary>
        /// Returns a list of games a player owns along with some playtime information, if the profile is publicly visible.
        /// </summary>
        /// <param name="steamid">The 64 bit ID of the player.</param>
        /// <param name="detailedInfo">Whether or not to include additional details of apps - name and images. Defaults to false.</param>
        /// <param name="includeFreeGames">Whether or not to list free-to-play games in the results. Defaults to false.</param>
        /// <param name="filter">Restricts results to the appids passed here; does not seem to work as of 31 Mar 2013.</param>
        public async Task<Result<OwnedGames>> GetOwnedGamesAsync(string steamid, bool detailedInfo = false, bool includeFreeGames = true, params uint[] filter)
        {
            return new OwnedGamesResponse(await SendRequestAsync(new OwnedGamesRequest(APIKey, steamid, detailedInfo, includeFreeGames, filter))).Result;
        }
        
        ////////////////////////////////////////////////////////////////////////////////
        /// Player's FamilyShare Status
        ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the original owner's SteamID if a borrowing account is currently playing this game. If the game is not borrowed or the borrower currently doesn't play this game, the result is null.
        /// </summary>
        /// <param name="steamid">The SteamID of the account playing.</param>
        /// <param name="appID">The AppID of the game currently playing</param>
        /// <returns></returns>
        public async Task<Result<FamilyShareAccount>> IsPlayingSharedGameAsync(string steamid, int appID)
        {
            return new FamilyShareAccountResponse(await SendRequestAsync(new FamilyShareAccountRequest(APIKey, steamid, appID))).Result;
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        private const int MaxSteamBatchRequestCount = 100;
    }
}
