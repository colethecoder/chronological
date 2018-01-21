using System;
using Chronological.QueryResults;

namespace Chronological.Exceptions
{
    internal class ErrorToExceptionConverter : IErrorToExceptionConverter
    {
        Exception IErrorToExceptionConverter.ConvertTimeSeriesErrorToException(ErrorResult error)
        {
            switch (error.Code)
            {
                case ("AuthenticationFailed"):
                    if (error.InnerError?.Code == "TokenExpired")
                    {
                        return new ChronologicalExpiredAccessTokenException(error.InnerError.Message);
                    }
                    return GenerateUnexpectedException(error);
                default:
                    return GenerateUnexpectedException(error);
            }                        
        }

        internal ChronologicalExpiredAccessTokenException GenerateExpiredAccessTokenException(ErrorResult error)
        {
            return new ChronologicalExpiredAccessTokenException(error.InnerError.Message);
        }

        internal ChronologicalUnexpectedException GenerateUnexpectedException(ErrorResult error)
        {
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
