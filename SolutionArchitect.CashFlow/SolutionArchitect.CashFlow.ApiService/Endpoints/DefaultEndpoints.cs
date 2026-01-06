using MediatR;
using Microsoft.AspNetCore.Mvc;
using SolutionArchitect.CashFlow.Api.Shareable.Requests;
using SolutionArchitect.CashFlow.Api.Shareable.Responses;

namespace SolutionArchitect.CashFlow.ApiService.Endpoints;

public static class DefaultEndpoints
{
    public static void MapAppEndpoints(this IEndpointRouteBuilder app)
        => app.MapPost("api/v1/cashflow", static async ([FromServices] IMediator mediator, [FromBody] ExecuteCashFlowOperationRequest request, CancellationToken cancellationToken) => await mediator.Send(request, cancellationToken))
            .WithName("CashFlow")
            .WithDisplayName("CashFlow")
            .WithTags("CashFlow")
            .Produces<ExecuteCashFlowOperationResponse>(200);
}