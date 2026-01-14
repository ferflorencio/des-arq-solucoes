using SolutionArchitect.CashFlow.Consolidate.Api.Application.Cache;
using SolutionArchitect.CashFlow.Consolidate.Api.Application.Handlers;
using SolutionArchitect.CashFlow.Consolidate.Api.Data.Cache;
using SolutionArchitect.CashFlow.Consolidate.ApiService.Endpoints;
using SolutionArchitect.CashFlow.Consolidate.ApiService.Extensions;
using SolutionArchitect.CashFlow.ServiceDefaults;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("redis")
        ?? throw new InvalidOperationException(
            "Redis connection string not found")));

// Cache abstraction
builder.Services.AddScoped<ICashFlowCache, RedisConsolidateCache>();

// MediatR
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

// Minimal APIs / Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(c =>
{
    c.AddDocumentTransformer((document, _, _) =>
    {
        document.Servers = [];
        document.Info = new()
        {
            Title = "SolutionArchitect.CashFlow.Consolidate.ApiService",
            Version = "v1",
            Description = "Aplicação Teste de Arquitetura de Soluções - CashFlow Consolidate",
        };

        return Task.CompletedTask;
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "SolutionArchitect.CashFlow.Consolidate.Api");
    });
}

app.UseApiExceptionHandling();

app.MapAppEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
