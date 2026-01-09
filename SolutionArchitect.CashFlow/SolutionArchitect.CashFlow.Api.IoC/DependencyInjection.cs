using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SolutionArchitect.CashFlow.Api.Data.Database;
using SolutionArchitect.CashFlow.Api.Data.Messaging;
using SolutionArchitect.CashFlow.Api.Data.Migrations;
using SolutionArchitect.CashFlow.Api.Data.Repository;
using SolutionArchitect.CashFlow.Api.Domain.Aggregates;
using SolutionArchitect.CashFlow.Api.Domain.Messaging;

namespace SolutionArchitect.CashFlow.Api.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<MongoOptions>()
            .Bind(configuration.GetSection("Mongo"))
            .PostConfigure(options =>
            {
                options.ConnectionString =
                    configuration.GetConnectionString("cashflow")
                    ?? throw new InvalidOperationException(
                        "Mongo connection string 'mongo' was not found.");
            });

        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp
                .GetRequiredService<IOptions<MongoOptions>>()
                .Value;

            return new MongoClient(options.ConnectionString);
        });

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<ICashFlowRepository, CashFlowRepository>();
        services.AddSingleton<MongoDbMigrations>();

        return services;
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

        return services;
    }
}