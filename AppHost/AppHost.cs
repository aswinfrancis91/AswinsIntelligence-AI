var builder = DistributedApplication.CreateBuilder(args);

var connectionString = builder.AddConnectionString("DefaultConnection");

var api = builder.AddProject<Projects.API>("AswinsIntelligenceApi").WithReference(connectionString);

var website = builder.AddNpmApp("react", "../ChatInterface", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "VITE_PORT", port: 5173)
    .WithExternalHttpEndpoints();

builder.Build().Run();