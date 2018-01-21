using System;
using Chronological.QueryResults;

namespace Chronological.Exceptions
{
    internal interface IErrorToExceptionConverter
    {
        Exception ConvertTimeSeriesErrorToException(ErrorResult error);
    }
}