namespace SolutionArchitect.CashFlow.Consolidate.Worker.Application.Messaging;

public sealed record CashFlowUpdatedEvent(
    DateTime Date,
    decimal NewBalance);