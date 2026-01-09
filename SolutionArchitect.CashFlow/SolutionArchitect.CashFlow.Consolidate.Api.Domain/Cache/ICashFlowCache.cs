namespace SolutionArchitect.CashFlow.Consolidate.Api.Domain.Cache;
public interface ICashFlowCache
{
    Task <string?> GetAsync(
        DateTime date,
        CancellationToken cancellationToken);
}