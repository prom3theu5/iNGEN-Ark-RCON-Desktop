using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.Extensions
{
    public static class ListExtensions
    {
        public static List<T[]> Split<T>(this List<T> list, int maxsize)
        {
            List<T[]> groups = new List<T[]>();

            int remaining = list.Count;
            int index = 0;
            while(remaining > 0)
            {
                int newGroupCount = maxsize < remaining ? maxsize : remaining;

                groups.Add(list.GetRange(index, newGroupCount).ToArray());

                remaining -= maxsize;
                index += newGroupCount;
            }

            return groups;
        }

        public static List<T[]> Split<T>(this IEnumerable<T> list, int maxsize)
        {
            List<T[]> groups = new List<T[]>();

            int remaining = list.Count();
            int index = 0;
            while(remaining > 0)
            {
                int newGroupCount = maxsize < remaining ? maxsize : remaining;

                groups.Add(list.Skip(index).Take(newGroupCount).ToArray());

                remaining -= maxsize;
                index += newGroupCount;
            }

            return groups;
        }
    }
}
