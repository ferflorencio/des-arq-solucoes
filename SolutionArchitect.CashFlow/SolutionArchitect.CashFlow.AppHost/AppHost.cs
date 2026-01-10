var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddMongoDB("mongodb").WithDataVolume().WithMongoExpress().AddDatabase("cashflow");

var redis = builder.AddRedis("redis").WithRedisInsight();

var rabbit = builder.AddRabbitMQ("rabbit").WithManagementPlugin();

var cashFlowService = builder.AddProject<Projects.SolutionArchitect_CashFlow_ApiService>("cashflow-api")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WithReference(rabbit)
    .WaitFor(db)
    .WaitFor(rabbit);

builder.AddProject<Projects.SolutionArchitect_CashFlow_FinancialConsolidate_Worker>("cashflow-consolidate-worker")
    .WithReference(redis)
    .WithReference(rabbit)
    .WaitFor(rabbit)
    .WaitFor(redis);

var consolidateService =  builder.AddProject<Projects.SolutionArchitect_CashFlow_Consolidate_ApiService>("cashflow-consolidate-api")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WaitFor(redis);

builder.AddProject<Projects.SolutionArchitect_CashFlow_Web>("cashflow-web")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(consolidateService)
    .WithReference(cashFlowService)
    .WaitFor(consolidateService)
    .WaitFor(cashFlowService);

builder.Build().Run();