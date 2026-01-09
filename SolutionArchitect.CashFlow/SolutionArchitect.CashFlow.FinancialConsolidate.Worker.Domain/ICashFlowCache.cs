namespace SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Domain;
public interface ICashFlowCache
{
    Task SetAsync(
        DateTime date,
        decimal balance,
        CancellationToken cancellationToken);
}