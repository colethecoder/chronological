using System;

namespace Chronological.Exceptions
{
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
