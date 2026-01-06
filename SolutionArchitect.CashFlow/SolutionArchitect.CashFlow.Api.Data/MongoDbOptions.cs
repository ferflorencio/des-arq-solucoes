namespace SolutionArchitect.CashFlow.Api.Data;

public sealed record MongoOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Database { get; init; } = default!;
}