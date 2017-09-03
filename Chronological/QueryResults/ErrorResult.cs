using System;
using System.Collections.Generic;
using System.Text;

namespace Chronological.QueryResults
{
    public class ErrorResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public ErrorResult InnerError { get; set; }
    }
}
