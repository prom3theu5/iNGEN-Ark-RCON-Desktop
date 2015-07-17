using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb
{
    public enum CommunityVisibilityState
    {
        Private = 1,
        Public = 3,
        FriendsOnly = 8
    }

    public enum PersonaState
    {
        OfflineOrPrivate = 0,
        Online = 1,
        Busy = 2,
        Away = 3,
        Snooze = 4,
        LookingToTrade = 5,
        LookingToPlay = 6,
    }

    public enum ProfileState
    {
        NotSetup,
        Setup,
    }
}
