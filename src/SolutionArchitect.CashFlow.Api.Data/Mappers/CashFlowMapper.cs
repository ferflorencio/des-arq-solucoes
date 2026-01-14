using SolutionArchitect.CashFlow.Api.Application.Aggregates;
using SolutionArchitect.CashFlow.Api.Shareable.Dtos;

namespace SolutionArchitect.CashFlow.Api.Data.Mappers;

public static class CashFlowMapper
{
    public static CashFlowAggregate ToDomain(CashFlowDocument doc)
        => CashFlowAggregate.Rehydrate(doc.Date, doc.Balance, doc.Version);

    public static CashFlowDocument ToDocument(CashFlowAggregate domain)
        => new()
        {
            Date = domain.Date,
            Balance = domain.Balance,
            Version = domain.Version
        };
}