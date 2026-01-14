namespace SolutionArchitect.CashFlow.Web.Contracts;

public sealed record ConsolidatedCashFlowResponse(
    DateTime Date,
    string Balance);
