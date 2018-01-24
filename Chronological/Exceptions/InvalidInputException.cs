using System;

namespace Chronological.Exceptions
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}
