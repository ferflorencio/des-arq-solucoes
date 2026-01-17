namespace SolutionArchitect.CashFlow.Consolidate.Worker.Data.Messaging;

public interface ICashFlowEventConsumer
{
    Task StartAsync(CancellationToken cancellationToken);
}