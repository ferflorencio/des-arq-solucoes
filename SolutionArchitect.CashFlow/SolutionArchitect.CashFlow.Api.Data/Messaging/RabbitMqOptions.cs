namespace SolutionArchitect.CashFlow.Api.Data.Messaging;

public sealed record RabbitMqOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Exchange { get; init; } = "cashflow.events";
}