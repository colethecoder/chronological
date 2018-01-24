using System;

namespace Chronological.Exceptions
{
    public class UnexpectedApiQueryException : Exception
    {
        public UnexpectedApiQueryException(string message) : base(message)
        {
        }
    }
}
