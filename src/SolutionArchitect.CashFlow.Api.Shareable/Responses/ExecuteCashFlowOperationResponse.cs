namespace SolutionArchitect.CashFlow.Api.Shareable.Responses;
public sealed record ExecuteCashFlowOperationResponse(
    decimal PreviousBalance,
    decimal NewBalance,
    string Operation
);