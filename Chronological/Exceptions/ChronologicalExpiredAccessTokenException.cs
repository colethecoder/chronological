using System;
using System.Collections.Generic;
using System.Text;

namespace Chronological.Exceptions
{
    public class ChronologicalExpiredAccessTokenException : Exception
    {
        public ChronologicalExpiredAccessTokenException(string message) : base(message)
        {
        }
    }
}
