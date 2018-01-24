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
                        return new TokenExpiredException(error.InnerError.Message);
                    }
                    return GenerateUnexpectedException(error);
                case ("InvalidInput"):
                    return GetInvalidInputException(error);
                default:
                    return GenerateUnexpectedException(error);
            }                        
        }

        private InvalidInputException GetInvalidInputException(ErrorResult error)
        {
            if (error.InnerError != null)
            {
                Exception innerException;
                switch (error.InnerError.Code)
                {
                    case ("PropertyNotFound"):
                        innerException = new PropertyNotFoundException(ConcatenateMessageAndTargetDetails(error.InnerError));
                        break;
                    default:
                        innerException = new UnexpectedApiQueryException(ConcatenateMessageAndTargetDetails(error.InnerError));
                        break;
                }
                return new InvalidInputException(ConcatenateMessageAndTargetDetails(error), innerException);
            }
            return new InvalidInputException(ConcatenateMessageAndTargetDetails(error), null);
        }

        internal TokenExpiredException GenerateTokenExpiredException(ErrorResult error)
        {
            return new TokenExpiredException(error.InnerError.Message);
        }

        internal UnexpectedApiQueryException GenerateUnexpectedException(ErrorResult error)
        {
            var errorMessage = $"Error Code: {error.Code}, Error Message: {error.Message}";
            if (error.InnerError != null)
            {
                errorMessage +=
                    $", Inner Error Code: {error.InnerError.Code}, Inner Error Message: {error.InnerError.Message}";
            }
            return new UnexpectedApiQueryException(errorMessage);
        }

        private string ConcatenateMessageAndTargetDetails(ErrorResult error)
        {
            if (string.IsNullOrWhiteSpace(error.Target))
            {
                return error.Message;
            }
            return $"{error.Message} Target: {error.Target}";
        }
    }
}
