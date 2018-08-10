using System;
using System.Collections.Generic;
using System.Text;
using Chronological.Exceptions;
using Chronological.QueryResults;
using Xunit;

namespace Chronological.Tests
{
    public class InvalidInputExceptionTests
    {
        [Fact]
        public void PropertyNotFoundErrorShouldReturnException()
        {
            var errorToException = new ErrorToExceptionConverter();
            var error = new ErrorResult { Code = "InvalidInput", Message = "Dimension property not found.", Target = "data.type", InnerError = new ErrorResult { Code = "PropertyNotFound", Message = "Dimension property 'data.type' of type 'String' is not found." } };
            var result = errorToException.ConvertTimeSeriesErrorToException(error);

            Assert.IsType<InvalidInputException>(result);
            Assert.IsType<PropertyNotFoundException>(result.InnerException);
            Assert.Equal("Dimension property not found. Target: data.type", result.Message);
            Assert.Equal("Dimension property 'data.type' of type 'String' is not found.", result.InnerException.Message);
        }

    }
}
