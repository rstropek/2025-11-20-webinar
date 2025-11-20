var builder = DistributedApplication.CreateBuilder(args);

var webapi = builder.AddProject<Projects.WebApi>("webapi");

// This is new in .NET 10: Vite & Python support
var frontend = builder.AddViteApp("frontend", "../Frontend")
    .WithReference(webapi)
    .WithExternalHttpEndpoints();

builder.Build().Run();
