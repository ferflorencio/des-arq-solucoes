using SolutionArchitect.CashFlow.Consolidate.Api.Application.Cache;
using StackExchange.Redis;

namespace SolutionArchitect.CashFlow.Consolidate.Api.Data.Cache;

public sealed class RedisConsolidateCache(IConnectionMultiplexer multiplexer) : ICashFlowCache
{
    private readonly IDatabase _database = multiplexer.GetDatabase();

    public async Task<string?> GetAsync(
        DateTime date,
        CancellationToken cancellationToken)
    {
        var key = $"cashflow:{date:yyyy-MM-dd}";

        return await _database.StringGetAsync(key); ;
    }
}
