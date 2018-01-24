namespace Chronological.QueryResults
{
    public class ErrorResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Target { get; set; }
        public ErrorResult InnerError { get; set; }
    }
}
