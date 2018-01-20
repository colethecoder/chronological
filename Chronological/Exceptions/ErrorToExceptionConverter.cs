using System;
using Chronological.QueryResults;

namespace Chronological.Exceptions
{
    internal interface IErrorToExceptionConverter
    {
        Exception ConvertTimeSeriesErrorToException(ErrorResult error);
    }

    internal class ErrorToExceptionConverter : IErrorToExceptionConverter
    {
        public Exception ConvertTimeSeriesErrorToException(ErrorResult error)
        {
            if (error.Code == "AuthenticationFailed")
            {
                if (error.InnerError?.Code == "TokenExpired")
                {
                    return new ChronologicalExpiredAccessTokenException(error.InnerError.Message);
                }
            }
            var errorMessage = $"Error Code: {error.Code}, Error Message: {error.Message}";
            if (error.InnerError != null)
            {
                errorMessage +=
                    $", Inner Error Code: {error.InnerError.Code}, Inner Error Message: {error.InnerError.Message}";
            }
            return new ChronologicalUnexpectedException(errorMessage);
        }
    }
}
