namespace SolutionArchitect.CashFlow.Web.Contracts;

public sealed record ExecuteCashFlowOperationRequest(
    string OperationType,
    decimal Amount
);
