using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.SteamInterface
{
    public abstract class SteamResponseBase<T>
    {
        public readonly Result<T> Result;

        public SteamResponseBase(Result<string> steamResponse)
        {
            if(steamResponse.IsOk)
                Result = Result<T>.Ok(Parse(steamResponse.Value));
            else
                Result = Result<T>.Err(steamResponse.ErrorMessage);
        }

        protected abstract T Parse(string result);
    }
}
