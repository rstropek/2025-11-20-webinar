var builder = DistributedApplication.CreateBuilder(args);

var webapi = builder.AddProject<Projects.WebApi>("webapi");

builder.Build().Run();
