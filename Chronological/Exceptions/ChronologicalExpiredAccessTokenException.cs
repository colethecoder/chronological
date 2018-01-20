using System;

namespace Chronological.Exceptions
{
    public class ChronologicalExpiredAccessTokenException : Exception
    {
        public ChronologicalExpiredAccessTokenException(string message) : base(message)
        {
        }
    }
}
