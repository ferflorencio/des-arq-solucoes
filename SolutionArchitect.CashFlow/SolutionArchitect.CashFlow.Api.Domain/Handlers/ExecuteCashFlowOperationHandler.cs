using MediatR;
using Polly;
using Polly.Registry;
using SolutionArchitect.CashFlow.Api.Domain.Aggregates;
using SolutionArchitect.CashFlow.Api.Domain.Config;
using SolutionArchitect.CashFlow.Api.Domain.Factories;
using SolutionArchitect.CashFlow.Api.Domain.Messaging;
using SolutionArchitect.CashFlow.Api.Shareable.Requests;
using SolutionArchitect.CashFlow.Api.Shareable.Responses;

namespace SolutionArchitect.CashFlow.Api.Domain.Handlers;

public sealed class ExecuteCashFlowOperationHandler(
    CashFlowOperationFactory factory,
    ICashFlowRepository repository,
    IEventPublisher publisher,
    ResiliencePipelineProvider<string> pipelineProvider) : IRequestHandler<ExecuteCashFlowOperationRequest, ExecuteCashFlowOperationResponse>
{
    private readonly ResiliencePipeline _pipeline = pipelineProvider.GetPipeline(ResilienceConfiguration.CashFlowPipeline);

    public async Task<ExecuteCashFlowOperationResponse> Handle(ExecuteCashFlowOperationRequest request, CancellationToken cancellationToken)
    {
        var today = DateTime.Now.Date;

        return await _pipeline.ExecuteAsync(
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