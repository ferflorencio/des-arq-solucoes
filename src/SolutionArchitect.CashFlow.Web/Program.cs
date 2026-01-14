using SolutionArchitect.CashFlow.ServiceDefaults;
using SolutionArchitect.CashFlow.Web.Components;
using SolutionArchitect.CashFlow.Web.Services.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.Services.AddOutputCache();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServiceDiscovery();

builder.Services.AddHttpClient<CashFlowConsolidateClient>(client =>
{
    var baseUrl =
        builder.Configuration["services:cashflow-consolidate-api:https:0"]
        ?? throw new InvalidOperationException("CashFlow Consolidate API endpoint not found");

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<CashFlowClient>(client =>
{
    var baseUrl =
        builder.Configuration["services:cashflow-api:https:0"]
        ?? throw new InvalidOperationException("CashFlow API not found");

    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
