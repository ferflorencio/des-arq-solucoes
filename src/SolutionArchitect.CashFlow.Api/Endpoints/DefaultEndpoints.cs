using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionArchitect.CashFlow.Api.Shareable.Requests;
using SolutionArchitect.CashFlow.Api.Shareable.Responses;

namespace SolutionArchitect.CashFlow.Api.Endpoints;

public static class DefaultEndpoints
{
    public static void MapAppEndpoints(this IEndpointRouteBuilder app)
        => app.MapPost(
            "api/v1/cashflow",
            static async (
                [FromServices] IMediator mediator,
                [FromBody] ExecuteCashFlowOperationRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(request, cancellationToken);
                return Results.Ok(response);
            })
        .WithName("CashFlow")
        .WithDisplayName("CashFlow")
        .WithTags("CashFlow")
        .Produces<ExecuteCashFlowOperationResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
}