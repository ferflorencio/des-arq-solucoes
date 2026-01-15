using SolutionArchitect.CashFlow.Consolidate.Api.Application.Cache;
using SolutionArchitect.CashFlow.Consolidate.Api.Application.Handlers;
using SolutionArchitect.CashFlow.Consolidate.Api.Data.Cache;
using SolutionArchitect.CashFlow.Consolidate.Api.Endpoints;
using SolutionArchitect.CashFlow.ServiceDefaults;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("redis")
        ?? throw new InvalidOperationException(
            "Redis connection string not found")));

builder.Services.AddScoped<ICashFlowCache, RedisConsolidateCache>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<
        GetDailyConsolidatedCashFlowHandler>());

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(c =>
{
    c.AddDocumentTransformer((document, _, _) =>
    {
        document.Servers = [];
        document.Info = new()
        {
            Title = "SolutionArchitect.CashFlow.Consolidate.Api",
            Version = "v1",
            Description = "Aplicação Teste de Arquitetura de Soluções - CashFlow Consolidate",
        };

        return Task.CompletedTask;
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "SolutionArchitect.CashFlow.Consolidate.Api");
    });
}

app.MapAppEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
