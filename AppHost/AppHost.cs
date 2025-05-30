using YamlDotNet.Core.Tokens;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.API>("AswinsIntelligenceApi");

var website = builder.AddNpmApp("react", "../ChatInterface", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "VITE_PORT",port: 5173)
    .WithExternalHttpEndpoints();

builder.Build().Run();