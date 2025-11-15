var builder = DistributedApplication.CreateBuilder(args);

var mongodb = builder
    .AddMongoDB("mongodb")
    .WithMongoExpress(containerName: "mongodb-express")
    .WithDataVolume();

var rabbitmq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume();

var mcp = builder
    .AddProject<Projects.MCP_Server>("mcp")
    .WithHttpHealthCheck("/healthz");

var agent = builder
    .AddPythonApp("agent", "../Services/Agent", "main.py")
    .WithUv()
    .WithEnvironment("ConnectionStrings__mcp", mcp.GetEndpoint("http"))
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

var gateway = builder
    .AddProject<Projects.Bot_Gateway>("gateway")
    .WithReference(mongodb)
    .WaitFor(mongodb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

await builder.Build().RunAsync();