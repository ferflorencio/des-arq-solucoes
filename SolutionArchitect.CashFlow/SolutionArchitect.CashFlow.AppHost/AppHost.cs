var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddMongoDB("mongodb").WithDataVolume().AddDatabase("cashflow");

var redis = builder.AddRedis("redis");

var rabbit = builder.AddRabbitMQ("rabbit").WithManagementPlugin();

var apiService = builder.AddProject<Projects.SolutionArchitect_CashFlow_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WithReference(rabbit)
    .WaitFor(db)
    .WaitFor(rabbit);

builder.AddProject<Projects.SolutionArchitect_CashFlow_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.SolutionArchitect_CashFlow_FinancialConsolidate_Worker>("cashflow-worker")
    .WithReference(redis)
    .WithReference(rabbit)
    .WaitFor(rabbit)
    .WaitFor(redis);

builder.Build().Run();