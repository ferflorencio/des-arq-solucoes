namespace SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Domain.Cache;
public interface ICashFlowCache
{
    Task SetAsync(
        DateTime date,
        decimal balance,
        CancellationToken cancellationToken);
}