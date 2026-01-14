using MediatR;
using SolutionArchitect.CashFlow.Api.Shareable.Enums;
using SolutionArchitect.CashFlow.Api.Shareable.Responses;

namespace SolutionArchitect.CashFlow.Api.Shareable.Requests;

public sealed record ExecuteCashFlowOperationRequest(CashFlowOperationType OperationType, decimal Amount) : IRequest<ExecuteCashFlowOperationResponse>;