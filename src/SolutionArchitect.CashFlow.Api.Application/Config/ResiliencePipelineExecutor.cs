namespace SolutionArchitect.CashFlow.Api.Application.Config
{
    public sealed class ResiliencePipelineExecutor(IResiliencePipelineRegistry registry) : IResiliencePipelineExecutor
    {
        private readonly IResiliencePipelineRegistry _registry = registry ?? throw new ArgumentNullException(nameof(registry));

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(action);

            var pipelineName = "cashflow-pipeline";

            if (!_registry.TryGet(pipelineName, out var policy) || policy == null)
            {
                return await action(cancellationToken).ConfigureAwait(false);
            }

            return await policy.ExecuteAsync(ct => action(ct), cancellationToken).ConfigureAwait(false);
        }
    }
}
