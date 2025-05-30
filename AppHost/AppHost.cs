using YamlDotNet.Core.Tokens;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.API>("AswinsIntelligenceApi");

// var website = builder.AddNpmApp("react", "../ChatInterface")
//     .WithReference(api)
//     .WaitFor(api)
//     .WithHttpsEndpoint();

var website = builder.AddNpmApp("react", "../ChatInterface", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();