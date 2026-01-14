namespace SolutionArchitect.CashFlow.Consolidate.Api.Application.Cache;
public interface ICashFlowCache
{
    Task <string?> GetAsync(
        DateTime date,
        CancellationToken cancellationToken);
}