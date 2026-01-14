using SolutionArchitect.CashFlow.Consolidate.Worker;
using SolutionArchitect.CashFlow.Consolidate.Worker.Application.Cache;
using SolutionArchitect.CashFlow.Consolidate.Worker.Data.Cache;
using SolutionArchitect.CashFlow.Consolidate.Worker.Data.Messaging;
using SolutionArchitect.CashFlow.ServiceDefaults;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.AddRabbitMQClient("rabbit");

builder.Services.Configure<RedisOptions>(options =>
{
    options.ConnectionString =
        builder.Configuration.GetConnectionString("redis")!;
});

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<RedisOptions>>().Value);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(
        sp.GetRequiredService<RedisOptions>().ConnectionString));

builder.Services.AddSingleton<ICashFlowCache, RedisCashFlowCache>();
builder.Services.AddSingleton<CashFlowEventConsumer>();
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
