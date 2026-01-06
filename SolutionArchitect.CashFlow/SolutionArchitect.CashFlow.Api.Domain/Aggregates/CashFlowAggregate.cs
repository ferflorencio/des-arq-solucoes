namespace SolutionArchitect.CashFlow.Api.Domain.Aggregates;

public sealed class CashFlowAggregate
{
    public DateTime Date { get; }
    public decimal Balance { get; private set; }
    public int Version { get; private set; }

    private CashFlowAggregate(DateTime date, decimal balance, int version)
    {
        Date = date;
        Balance = balance;
        Version = version;
    }

    public static CashFlowAggregate Create(DateTime date)
        => new(date, 0m, 0);

    internal static CashFlowAggregate Rehydrate(
        DateTime date,
        decimal balance,
        int version)
        => new(date, balance, version);

    public void Apply(decimal newBalance)
    {
        Balance = newBalance;
    }
}