using MediatR;
using SolutionArchitect.CashFlow.Api.Application.Aggregates;
using SolutionArchitect.CashFlow.Api.Application.Config;
using SolutionArchitect.CashFlow.Api.Application.Factories;
using SolutionArchitect.CashFlow.Api.Application.Messaging;
using SolutionArchitect.CashFlow.Api.Shareable.Requests;
using SolutionArchitect.CashFlow.Api.Shareable.Responses;

namespace SolutionArchitect.CashFlow.Api.Application.Handlers;

public sealed class ExecuteCashFlowOperationHandler(
    ICashFlowOperationFactory factory,
    ICashFlowRepository repository,
    IEventPublisher publisher,
    IResiliencePipelineExecutor executor) : IRequestHandler<ExecuteCashFlowOperationRequest, ExecuteCashFlowOperationResponse>
{
    private readonly IResiliencePipelineExecutor _executor = executor;

    public async Task<ExecuteCashFlowOperationResponse> Handle(ExecuteCashFlowOperationRequest request, CancellationToken cancellationToken)
    {
        var today = DateTime.Now.Date;

        return await _executor.ExecuteAsync(
            async token =>
            {
                var aggregate = await repository.GetByDateAsync(today, token) ?? CashFlowAggregate.Create(today);

                var previousBalance = aggregate.Balance;

                var operation = factory.Create(request.OperationType);
                var newBalance = operation.Execute(previousBalance, request.Amount);

                aggregate.Apply(newBalance);

                await repository.TrySaveAsync(aggregate, token);

                await publisher.PublishAsync(new CashFlowUpdatedEvent(Date: today, NewBalance: newBalance), token);

                return new ExecuteCashFlowOperationResponse(previousBalance, newBalance, request.OperationType.ToString());
            },
            cancellationToken);
    }
}