using SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Domain;
using StackExchange.Redis;

namespace SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Data.Cache;

public sealed class RedisCashFlowCache(IConnectionMultiplexer multiplexer) : ICashFlowCache
{
    private readonly IDatabase _database = multiplexer.GetDatabase();

    public async Task SetAsync(
        DateTime date,
        decimal balance,
        CancellationToken cancellationToken)
    {
        var key = $"cashflow:{date:yyyy-MM-dd}";

        await _database.StringSetAsync(
            key,
            balance.ToString());
    }
}
