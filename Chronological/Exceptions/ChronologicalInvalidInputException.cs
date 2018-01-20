using System;

namespace Chronological.Exceptions
{
    public class ChronologicalInvalidInputException : Exception
    {
        public ChronologicalInvalidInputException(string message) : base(message)
        {
        }
    }
}
