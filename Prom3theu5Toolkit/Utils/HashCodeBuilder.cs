using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTK.Utils
{
    /// <summary>
    /// Helps build a hashcode comprised of multiple fields.
    /// </summary>
    public class HashCodeBuilder
    {
        private int Result {get; set;}

        /// <summary>
        /// </summary>
        /// <param name="field">The first field.</param>
        public HashCodeBuilder(object field)
        {
            Result = field.GetHashCode();
        }

        /// <summary>
        /// Add a field to the hashcode.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
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
