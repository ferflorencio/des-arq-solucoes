using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Requests;
using SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Responses;
using SolutionArchitect.CashFlow.Consolidate.ApiService.Extensions;

namespace SolutionArchitect.CashFlow.Consolidate.ApiService.Endpoints;

public static class DefaultEndpoints
{
    public static void MapAppEndpoints(this IEndpointRouteBuilder app) => 
        app.MapGet("api/v1/cashflow/consolidated/{year:int}/{month:int}/{day:int}",
            async ([FromRoute] int year, [FromRoute] int month, [FromRoute] int day, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var request = new GetDailyConsolidatedCashFlowRequest(year, month, day);

                var response = await mediator.SendQueryOrNotFound(
                    request,
                    $"No cashflow found for {year:D4}-{month:D2}-{day:D2}",
                    cancellationToken);

                return Results.Ok(response);
            })
        .WithName("GetDailyCashFlowConsolidated")
        .WithDisplayName("Get Daily CashFlow Consolidated")
        .WithTags("Consolidate")
        .Produces<GetDailyConsolidatedCashFlowResponse>(200)
        .ProducesProblem(404)
        .ProducesProblem(422)
        .ProducesProblem(500);
}