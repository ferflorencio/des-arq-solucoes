using SolutionArchitect.CashFlow.Api.Data.Migrations;
using SolutionArchitect.CashFlow.Api.Domain.Config;
using SolutionArchitect.CashFlow.Api.Domain.Factories;
using SolutionArchitect.CashFlow.Api.Domain.Handlers;
using SolutionArchitect.CashFlow.Api.IoC;
using SolutionArchitect.CashFlow.ApiService.Endpoints;
using SolutionArchitect.CashFlow.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ExecuteCashFlowOperationHandler>());
builder.Services.AddInfrastructure(builder.Configuration);
builder.AddRabbitMQClient("rabbit");
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

builder.Services.AddScoped<CashFlowOperationFactory>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var indexInitializer =
        scope.ServiceProvider.GetRequiredService<MongoDbMigrations>();

    await indexInitializer.CreateIndexesAsync(CancellationToken.None);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/openapi/v1.json", "SolutionArchitect.CashFlow.ApiService");
    });
}

app.MapDefaultEndpoints();
app.MapAppEndpoints();
app.Run();