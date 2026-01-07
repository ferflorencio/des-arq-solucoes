namespace SolutionArchitect.CashFlow.Api.Domain.Messaging;
public sealed record CashFlowUpdatedEvent(
    DateTime Date,
    decimal NewBalance
);
