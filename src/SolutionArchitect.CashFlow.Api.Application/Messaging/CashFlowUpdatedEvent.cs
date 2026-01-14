namespace SolutionArchitect.CashFlow.Api.Application.Messaging;
public sealed record CashFlowUpdatedEvent(
    DateTime Date,
    decimal NewBalance
);
