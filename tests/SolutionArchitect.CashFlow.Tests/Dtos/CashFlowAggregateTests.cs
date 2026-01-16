using System;
using Xunit;
using SolutionArchitect.CashFlow.Api.Application.Aggregates;

namespace SolutionArchitect.CashFlow.Api.Tests.Dtos
{
    public class CashFlowAggregateTests
    {
        [Fact]
        public void Create_SetsDefaultValues()
        {
            var date = new DateTime(2025, 1, 1);
            var aggregate = CashFlowAggregate.Create(date);

            Assert.Equal(date, aggregate.Date);
            Assert.Equal(0m, aggregate.Balance);
            Assert.Equal(0, aggregate.Version);
        }
    }
}