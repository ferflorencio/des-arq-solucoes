namespace SolutionArchitect.CashFlow.Consolidate.Worker.Data.Cache;

public sealed record RedisOptions
{
    public string ConnectionString { get; set; } = default!;
}
