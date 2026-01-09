var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddMongoDB("mongodb").WithDataVolume().AddDatabase("cashflow");

var redis = builder.AddRedis("redis");

var rabbit = builder.AddRabbitMQ("rabbit").WithManagementPlugin();

var apiService = builder.AddProject<Projects.SolutionArchitect_CashFlow_ApiService>("cashflow-api")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WithReference(rabbit)
    .WaitFor(db)
    .WaitFor(rabbit);

builder.AddProject<Projects.SolutionArchitect_CashFlow_Web>("cashflow-web")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.SolutionArchitect_CashFlow_FinancialConsolidate_Worker>("cashflow-consolidate-worker")
    .WithReference(redis)
    .WithReference(rabbit)
    .WaitFor(rabbit)
    .WaitFor(redis);

builder.AddProject<Projects.SolutionArchitect_CashFlow_Consolidate_ApiService>("cashflow-consolidate-api")
    .WithReference(redis)
    .WaitFor(redis);

builder.Build().Run();