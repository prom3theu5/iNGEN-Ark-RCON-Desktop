using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class HashSetExtensions
    {
        public static void Add<T>(this HashSet<T> hashset, IEnumerable<T> items)
        {
            foreach(var item in items)
            {
                hashset.Add(item);
            }
        }

        public static void AddOrReplace<T>(this HashSet<T> hashset, T item)
        {
            hashset.Remove(item);
            hashset.Add(item);
        }

        public static void AddOrReplace<T>(this HashSet<T> hashset, IEnumerable<T> items)
        {
            foreach(var item in items)
            {
                hashset.Remove(item);
                hashset.Add(item);
            }
        }
    }
}
