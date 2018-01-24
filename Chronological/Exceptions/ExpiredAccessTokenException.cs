using System;

namespace Chronological.Exceptions
{
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException(string message) : base(message)
        {
        }
    }
}
