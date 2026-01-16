using System;
using System.Linq;
using Xunit;
using MediatR;
using SolutionArchitect.CashFlow.Api.Shareable.Requests;
using SolutionArchitect.CashFlow.Api.Shareable.Enums;
using SolutionArchitect.CashFlow.Api.Shareable.Responses;

namespace SolutionArchitect.CashFlow.Api.Tests.Requests
{
    public class ExecuteCashFlowOperationRequestTests
    {
        [Fact]
        public void Initialization_SetsProperties()
        {
            var request = new ExecuteCashFlowOperationRequest(CashFlowOperationType.Credit, 123.45m);

            Assert.Equal(CashFlowOperationType.Credit, request.OperationType);
            Assert.Equal(123.45m, request.Amount);
        }

        [Fact]
        public void Implements_IRequestOfExecuteCashFlowOperationResponse()
        {
            var interfaceType = typeof(ExecuteCashFlowOperationRequest)
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            Assert.NotNull(interfaceType);
            var genericArg = interfaceType.GetGenericArguments().First();
            Assert.Equal(typeof(ExecuteCashFlowOperationResponse), genericArg);
        }
    }
}