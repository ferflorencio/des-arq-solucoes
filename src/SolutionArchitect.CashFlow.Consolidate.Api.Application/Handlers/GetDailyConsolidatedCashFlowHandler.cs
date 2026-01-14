using MediatR;
using SolutionArchitect.CashFlow.Consolidate.Api.Application.Cache;
using SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Requests;
using SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Responses;

namespace SolutionArchitect.CashFlow.Consolidate.Api.Application.Handlers;

public sealed class GetDailyConsolidatedCashFlowHandler(ICashFlowCache cache) : IRequestHandler<GetDailyConsolidatedCashFlowRequest, GetDailyConsolidatedCashFlowResponse?>
{
    public async Task<GetDailyConsolidatedCashFlowResponse?> Handle(GetDailyConsolidatedCashFlowRequest request, CancellationToken cancellationToken)
    {
        var date = new DateTime(request.Year, request.Month, request.Day, 0, 0, 0, DateTimeKind.Utc);
        var balance = await cache.GetAsync(date, cancellationToken);

        return balance is null
            ? null
            : new GetDailyConsolidatedCashFlowResponse(date, balance);
    }
}
