using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.Utils
{
    /// <summary>
    /// Helps build a hashcode comprised of multiple fields.
    /// </summary>
    public class HashCodeBuilder
    {
        private int Result {get; set;}

        public HashCodeBuilder(object field)
        {
            Result = field.GetHashCode();
        }

        public HashCodeBuilder Add(object field)
        {
            unchecked
            {
                Result = (Result * 397) ^ field.GetHashCode();
            }

            return this;
        }

        public override int GetHashCode()
        {
            return Result;
        }
    }
}
