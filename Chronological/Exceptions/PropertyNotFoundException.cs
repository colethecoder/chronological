using System;

namespace Chronological.Exceptions
{
    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(string message) : base(message)
        {
        }
    }
}
