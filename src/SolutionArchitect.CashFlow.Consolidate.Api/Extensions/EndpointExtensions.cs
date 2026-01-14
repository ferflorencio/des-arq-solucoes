using MediatR;
using SolutionArchitect.CashFlow.Consolidate.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Consolidate.ApiService.Extensions;
public static class EndpointExtensions
{
    public static async Task<TResponse> SendQueryOrNotFound<TResponse>(
    this IMediator mediator,
    IRequest<TResponse?> request,
    string notFoundMessage,
    CancellationToken cancellationToken)
    where TResponse : class
    {
        var response = await mediator.Send(request, cancellationToken);

        return response is null ? throw new NotFoundException(notFoundMessage) : response;
    }
}


