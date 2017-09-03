using System;
using System.Collections.Generic;
using System.Text;

namespace Chronological.Exceptions
{
    public class ChronologicalUnexpectedException : Exception
    {
        public ChronologicalUnexpectedException(string message) : base(message)
        {
        }
    }
}
