var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.aspire92_ApiService>("apiservice")
    .WithHttpsHealthCheck("/health");

var webfronted = builder.AddProject<Projects.aspire92_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    .WithReference(apiService.GetEndpoint("http")) // Only reference HTTP.
    .WaitFor(apiService);

var bpchost = builder.AddProject<Projects.BIS_BPC_WebHost_Net9>("bis-bpc-webhost-net9")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    ;

builder.AddProject<Projects.aspire92_YarpProxy>("yarp")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    .WithReference(apiService.GetEndpoint("http")) // Only reference HTTP.
    .WithReference(webfronted.GetEndpoint("http")) // Only reference HTTP.
    .WithReference(bpchost.GetEndpoint("http"))
    .WaitFor(apiService)
    .WaitFor(webfronted)
    .WaitFor(bpchost)
    ;

builder.AddDockerComposePublisher();

await builder.Build().RunAsync();
