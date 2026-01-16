using FluentAssertions;
using MongoDB.Bson;
using SolutionArchitect.CashFlow.Api.Application.Aggregates;
using SolutionArchitect.CashFlow.Api.Data.Mappers;
using SolutionArchitect.CashFlow.Api.Shareable.Dtos;
using System;
using System.Reflection;

namespace SolutionArchitect.CashFlow.Api.Tests.Data.Mappers
{
    public class CashFlowMapperTests
    {
        private static CashFlowAggregate CreateAggregateViaReflection(DateTime date, decimal balance, int version)
        {
            var type = typeof(CashFlowAggregate);
            var method = type.GetMethod("Rehydrate", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return method is null
                ? throw new InvalidOperationException("Rehydrate method not found on CashFlowAggregate")
                : (CashFlowAggregate)method.Invoke(null, [date, balance, version])!;
        }

        [Fact]
        public void ToDomain_MapsDocumentToAggregate()
        {
            var doc = new CashFlowDocument
            {
                Id = ObjectId.GenerateNewId(),
                Date = new DateTime(2026, 1, 15),
                Balance = 500.25m,
                Version = 3
            };

            var aggregate = CashFlowMapper.ToDomain(doc);

            aggregate.Should().NotBeNull();
            aggregate.Date.Should().Be(doc.Date);
            aggregate.Balance.Should().Be(doc.Balance);
            aggregate.Version.Should().Be(doc.Version);
        }

        [Fact]
        public void ToDocument_MapsAggregateToDocument()
        {
            var date = new DateTime(2026, 1, 14);
            var aggregate = CreateAggregateViaReflection(date, 250.75m, 2);

            var doc = CashFlowMapper.ToDocument(aggregate);

            doc.Should().NotBeNull();
            doc.Date.Should().Be(aggregate.Date);
            doc.Balance.Should().Be(aggregate.Balance);
            doc.Version.Should().Be(aggregate.Version);

            doc.Id.Should().Be(ObjectId.Empty);
        }
    }
}