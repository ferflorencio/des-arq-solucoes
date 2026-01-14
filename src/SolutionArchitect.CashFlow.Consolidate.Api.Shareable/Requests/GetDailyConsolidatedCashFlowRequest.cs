using MediatR;
using SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Responses;

namespace SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Requests;
public sealed record GetDailyConsolidatedCashFlowRequest(int Year, int Month, int Day) : IRequest<GetDailyConsolidatedCashFlowResponse>;