using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddMongoDB("mongodb").WithDataVolume().AddDatabase("cashflow");


var rabbit = builder.AddRabbitMQ("rabbit").WithManagementPlugin();

var apiService = builder.AddProject<Projects.SolutionArchitect_CashFlow_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WithReference(rabbit);

builder.AddProject<Projects.SolutionArchitect_CashFlow_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.SolutionArchitect_CashFlow_FinancialConsolidate_Worker>("solutionarchitect-cashflow-financialconsolidate-worker");

builder.Build().Run();