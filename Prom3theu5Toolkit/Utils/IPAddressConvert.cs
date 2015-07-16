using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK.Utils
{
    public static class IPAddressConvert
    {
        public static uint BytesToDecimal(uint w, uint x, uint y, uint z)
        {
            return (16777216 * w) + (65536 * x) + (256 * y) + z;
        }

        public static uint StringToDecimal(string ip)
        {
            var splitIP = ip.Split('.');
            return BytesToDecimal(uint.Parse(splitIP[0]), uint.Parse(splitIP[1]), uint.Parse(splitIP[2]), uint.Parse(splitIP[3]));
        }

        public static string DecimalToString(uint ip)
        {
            return
                String.Format("{0}.{1}.{2}.{3}",
                ((ip / 16777216) % 256).ToString(),
                ((ip / 65536) % 256).ToString(),
                ((ip / 256) % 256).ToString(),
                ((ip) % 256).ToString());
        }

        public static bool IsValidIPAddress(string ip)
        {
            return System.Net.IPAddress.Parse(ip) != null;
        }
    }
}
