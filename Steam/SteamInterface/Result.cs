using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.SteamInterface
{
    public class Result<T>
    {
        public readonly T Value;
        public readonly bool IsOk;
        public readonly string ErrorMessage;

        public Result(T value)
        {
            Value = value;
            IsOk = true;
            ErrorMessage = string.Empty;
        }

        public Result(string errorMessage)
        {
            Value = default(T);
            IsOk = false;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Ok(T value)
        {
            return new Result<T>(value);
        }

        public static Result<T> Err(string errorMessage)
        {
            return new Result<T>(errorMessage);
        }
    }
}
