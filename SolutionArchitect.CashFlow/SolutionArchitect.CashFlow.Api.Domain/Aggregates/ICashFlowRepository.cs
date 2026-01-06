namespace SolutionArchitect.CashFlow.Api.Domain.Aggregates
{
    public interface ICashFlowRepository
    {
        Task<CashFlowAggregate?> GetByDateAsync(DateTime date, CancellationToken cancellationToken);

        Task TrySaveAsync(CashFlowAggregate aggregate, CancellationToken cancellationToken);
    }
}
