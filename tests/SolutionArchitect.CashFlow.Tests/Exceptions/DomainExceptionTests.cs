using System.Linq;
using Xunit;
using SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Api.Tests.Exceptions
{
    public class DomainExceptionTests
    {
        [Fact]
        public void RequestDataInvalidException_HasErrorsAndMessage()
        {
            var errors = new[] { "field1 is required", "field2 must be positive" };
            var ex = new RequestDataInvalidException(errors);

            Assert.Equal("Invalid request data", ex.Message);
            Assert.NotNull(ex.Errors);
            Assert.Equal(2, ex.Errors.Count);
            Assert.True(errors.SequenceEqual(ex.Errors));
        }

        [Fact]
        public void NotFoundException_PreservesMessage()
        {
            var message = "Resource not found";
            var ex = new NotFoundException(message);

            Assert.Equal(message, ex.Message);
        }

        [Fact]
        public void AppException_PreservesMessage()
        {
            var message = "Business rule violated";
            var ex = new AppException(message);

            Assert.Equal(message, ex.Message);
        }

        [Fact]
        public void LockException_PreservesMessage()
        {
            var message = "Concurrent update detected";
            var ex = new LockException(message);

            Assert.Equal(message, ex.Message);
        }

        [Fact]
        public void InvalidOperationTypeException_FormatsMessage()
        {
            var op = "UNKNOWN";
            var ex = new InvalidOperationTypeException(op);

            Assert.Equal($"Invalid operation type: {op}", ex.Message);
        }
    }
}