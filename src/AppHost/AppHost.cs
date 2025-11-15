var builder = DistributedApplication.CreateBuilder(args);

var mongodb = builder
    .AddMongoDB("mongodb")
    .WithMongoExpress(containerName: "mongodb-express")
    .WithDataVolume();

var rabbitmq = builder
    .AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume();

var gateway = builder
    .AddProject<Projects.Bot_Gateway>("gateway")
    .WithHttpHealthCheck("/healthz")
    .WithReference(mongodb)
    .WaitFor(mongodb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

var agent = builder
    .AddPythonApp("agent", "../Services/Agent", "main.py")
    .WithUv()
    .WithEnvironment("ConnectionStrings__mcp", gateway.GetEndpoint("http"))
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

await builder.Build().RunAsync();