using SolutionArchitect.CashFlow.Consolidate.Worker.Data.Messaging;

namespace SolutionArchitect.CashFlow.Consolidate.Worker;

public sealed class Worker(
    ILogger<Worker> logger,
    CashFlowEventConsumer consumer)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation("CashFlow Worker iniciado.");

        await consumer.StartAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}