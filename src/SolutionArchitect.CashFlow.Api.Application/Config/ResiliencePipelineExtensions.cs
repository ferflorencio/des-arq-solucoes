using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace SolutionArchitect.CashFlow.Api.Application.Config
{
    public enum DelayBackoffType { Constant, Exponential }

    public class RetryStrategyOptions
    {
        public int MaxRetryAttempts { get; set; } = 3;
        public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(50);
        public DelayBackoffType BackoffType { get; set; } = DelayBackoffType.Constant;
        public bool UseJitter { get; set; } = false;
        public Func<Exception, bool>? ShouldHandle { get; set; }
    }

    public sealed class PredicateBuilder
    {
        public Func<Exception, bool> Handle<TException>() where TException : Exception
            => ex => ex is TException;
    }

    public interface IResiliencePipelineRegistry
    {
        void Add(string name, IAsyncPolicy policy);
        bool TryGet(string name, out IAsyncPolicy? policy);
    }

    public sealed class ResiliencePipelineRegistry : IResiliencePipelineRegistry
    {
        private readonly ConcurrentDictionary<string, IAsyncPolicy> _policies = new();
        public void Add(string name, IAsyncPolicy policy) => _policies[name] = policy;
        public bool TryGet(string name, out IAsyncPolicy? policy) => _policies.TryGetValue(name, out policy);
    }

    public sealed class ResiliencePipelineBuilder
    {
        private readonly List<IAsyncPolicy> _policies = [];
        private readonly Random _random = new();

        public void AddRetry(RetryStrategyOptions options)
        {
            var shouldHandle = options.ShouldHandle ?? (_ => true);
            var policyHandle = Policy.Handle<Exception>(ex => shouldHandle(ex));

            IAsyncPolicy retryPolicy = Policy.NoOpAsync();
            if (options.MaxRetryAttempts > 0)
            {
                retryPolicy = policyHandle.WaitAndRetryAsync(
                    retryCount: options.MaxRetryAttempts,
                    sleepDurationProvider: attempt =>
                    {
                        double baseMs = options.BackoffType == DelayBackoffType.Exponential
                            ? options.Delay.TotalMilliseconds * Math.Pow(2, attempt - 1)
                            : options.Delay.TotalMilliseconds;

                        if (options.UseJitter)
                        {
                            var jitter = (_random.NextDouble() - 0.5) * 0.4 * baseMs;
                            baseMs = Math.Max(1, baseMs + jitter);
                        }

                        return TimeSpan.FromMilliseconds(baseMs);
                    });
            }

            _policies.Add(retryPolicy);
        }

        internal IAsyncPolicy BuildPolicy()
        {
            if (_policies.Count == 0) return Policy.NoOpAsync();
            if (_policies.Count == 1) return _policies[0];
            return Policy.WrapAsync([.. _policies]);
        }
    }

    public static class ResiliencePipelineExtensions
    {
        public static IServiceCollection AddResiliencePipeline(this IServiceCollection services, string pipelineName, Action<ResiliencePipelineBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);

            if (!services.Any(sd => sd.ServiceType == typeof(IResiliencePipelineRegistry)))
            {
                services.AddSingleton<IResiliencePipelineRegistry, ResiliencePipelineRegistry>();
            }

            var builder = new ResiliencePipelineBuilder();
            configure(builder);
            var policy = builder.BuildPolicy();

            services.AddSingleton<ResiliencePipelineInitializer>(sp =>
            {
                var r = sp.GetRequiredService<IResiliencePipelineRegistry>();
                r.Add(pipelineName, policy);
                return new ResiliencePipelineInitializer();
            });

            return services;
        }
    }

    internal sealed class ResiliencePipelineInitializer { }
}