using SolutionArchitect.CashFlow.Api.Application.Aggregates;

namespace SolutionArchitect.CashFlow.Api.Application.Aggregates
{
    public interface ICashFlowRepository
    {
        Task<CashFlowAggregate?> GetByDateAsync(DateTime date, CancellationToken cancellationToken);

        Task TrySaveAsync(CashFlowAggregate aggregate, CancellationToken cancellationToken);
    }
}
