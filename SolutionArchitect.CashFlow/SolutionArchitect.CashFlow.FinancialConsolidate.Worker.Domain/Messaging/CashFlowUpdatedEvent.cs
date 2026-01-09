namespace SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Domain.Messaging;

public sealed record CashFlowUpdatedEvent(
    DateTime Date,
    decimal NewBalance);