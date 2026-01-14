using SolutionArchitect.CashFlow.Api.Data.Migrations;
using SolutionArchitect.CashFlow.Api.Application.Config;
using SolutionArchitect.CashFlow.Api.Application.Factories;
using SolutionArchitect.CashFlow.Api.Application.Handlers;
using SolutionArchitect.CashFlow.Api.IoC;
using SolutionArchitect.CashFlow.ApiService.Endpoints;
using SolutionArchitect.CashFlow.ApiService.Extensions;
using SolutionArchitect.CashFlow.ServiceDefaults;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ExecuteCashFlowOperationHandler>());
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMessaging();
builder.Services.AddCashFlowResilience();

builder.Services.AddOpenApi(c =>
{
    c.AddDocumentTransformer((document, _, _) =>
    {
        document.Servers = [];
        document.Info = new()
        {
            Title = "SolutionArchitect.CashFlow.ApiService",
            Version = "v1",
            Description = "Aplicação Teste de Arquitetura de Soluções - CashFlow",
        };

        return Task.CompletedTask;
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    };
});

builder.Services.AddScoped<CashFlowOperationFactory>();
builder.AddRabbitMQClient("rabbit");


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var indexInitializer =
        scope.ServiceProvider.GetRequiredService<MongoDbMigrations>();

    await indexInitializer.CreateIndexesAsync(CancellationToken.None);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/openapi/v1.json", "SolutionArchitect.CashFlow.ApiService");
    });
}
app.UseDomainExceptionHandling();

app.MapDefaultEndpoints();
app.MapAppEndpoints();
app.Run();