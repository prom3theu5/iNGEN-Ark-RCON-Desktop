using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.SteamInterface
{
    public abstract class SteamRequestBase
    {
        public readonly string InterfaceName;
        public readonly string MethodName;
        public readonly int MethodVersion;
        public readonly bool RequiresSteamAPIKey;
        public readonly string SteamAPIKey;
        public readonly string Parameters;

        public SteamRequestBase(string interfaceName, string methodName, int methodVersion, string steamAPIKey, params string[] parameters):
            this(interfaceName, methodName, methodVersion, parameters)
        {
            SteamAPIKey = steamAPIKey;
            RequiresSteamAPIKey = true;
        }

        public SteamRequestBase(string interfaceName, string methodName, int methodVersion, params string[] parameters)
        {
            InterfaceName = interfaceName;
            MethodName = methodName;
            MethodVersion = methodVersion;
            Parameters = String.Join("&", parameters);
        }

        public string Request
        {
            get
            {
                string formattedParams = string.Empty;

                if(RequiresSteamAPIKey)
                    formattedParams = string.Format("key={0}&", SteamAPIKey);

                formattedParams += Parameters;

                return String.Format(SteamRequestTemplate, InterfaceName, MethodName, MethodVersion.ToString(), formattedParams);
            }
        }

        private const string SteamRequestTemplate = @"https://api.steampowered.com/{0}/{1}/v000{2}/?{3}&format=json";
    }
}
