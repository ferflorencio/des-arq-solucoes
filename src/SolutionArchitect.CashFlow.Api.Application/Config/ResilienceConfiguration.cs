using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using SolutionArchitect.CashFlow.Api.Shareable.Exceptions;

namespace SolutionArchitect.CashFlow.Api.Application.Config
{
    public static class ResilienceConfiguration
    {
        public const string CashFlowPipeline = "cashflow-pipeline";

        public static IServiceCollection AddCashFlowResilience(
            this IServiceCollection services)
        {
            services.AddResiliencePipeline(
                CashFlowPipeline,
                pipeline =>
                {
                    pipeline.AddRetry(new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromMilliseconds(50),
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,

                        ShouldHandle = new PredicateBuilder()
                            .Handle<LockException>()
                    });
                });

            return services;
        }
    }
}