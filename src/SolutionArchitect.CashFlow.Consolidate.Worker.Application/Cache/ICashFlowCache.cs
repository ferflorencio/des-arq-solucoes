namespace SolutionArchitect.CashFlow.Consolidate.Worker.Application.Cache;
public interface ICashFlowCache
{
    Task SetAsync(
        DateTime date,
        decimal balance,
        CancellationToken cancellationToken);
}