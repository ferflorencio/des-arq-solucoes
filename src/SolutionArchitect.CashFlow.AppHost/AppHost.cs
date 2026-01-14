var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddMongoDB("mongodb").WithDataVolume().WithMongoExpress().AddDatabase("cashflow");

var redis = builder.AddRedis("redis").WithRedisInsight();

var rabbit = builder.AddRabbitMQ("rabbit").WithManagementPlugin();

var cashFlowService = builder.AddProject<Projects.SolutionArchitect_CashFlow_Api>("cashflow-api")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WithReference(rabbit)
    .WaitFor(db)
    .WaitFor(rabbit);

builder.AddProject<Projects.SolutionArchitect_CashFlow_Consolidate_Worker>("cashflow-consolidate-worker")
    .WithReference(redis)
    .WithReference(rabbit)
    .WaitFor(rabbit)
    .WaitFor(redis);

var consolidateService =  builder.AddProject<Projects.SolutionArchitect_CashFlow_Consolidate_Api>("cashflow-consolidate-api")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WaitFor(redis);

var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

var scriptsPath = Path.Combine(solutionRoot, "tests", "k6");

var k6 = builder.AddK6("k6")
    .WithBindMount(scriptsPath, "/scripts", true)
    .WithScript("/scripts/consolidated-get-load-tests.js")
    .WithReference(consolidateService)
    .WithReference(cashFlowService)
    .WithEnvironment("BASE_URL_CONSOLIDATE", consolidateService.GetEndpoint("http"))
    .WithEnvironment("BASE_URL_CASHFLOW", cashFlowService.GetEndpoint("http"))
    .WithEnvironment("K6_WEB_DASHBOARD", "true")
    .WithEnvironment("K6_WEB_DASHBOARD_EXPORT", "dashboard-report.html")
    .WithHttpEndpoint(
        targetPort: 5665,
        name: "k6-dashboard"
    )
    .WithUrlForEndpoint("k6-dashboard", url => url.DisplayText = "K6 Dashboard")
    .WithK6OtlpEnvironment()
    .WaitFor(cashFlowService)
    .WaitFor(consolidateService)
    .WithExplicitStart();

builder.Build().Run();