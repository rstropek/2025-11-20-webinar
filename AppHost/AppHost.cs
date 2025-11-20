var builder = DistributedApplication.CreateBuilder(args);

var sqlite = builder.AddSqlite(
    "database",
    builder.Configuration["Database:path"],
    builder.Configuration["Database:fileName"])
    .WithSqliteWeb(); // optionally add web admin UI (requires Docker/podman)

var webapi = builder.AddProject<Projects.WebApi>("webapi")
    .WithEnvironment("ConnectionStrings__Database", $"Data Source={Path.Combine(
        builder.Configuration["Database:path"]!, 
        builder.Configuration["Database:fileName"]!)}");

// This is new in .NET 10: Vite & Python support
var frontend = builder.AddViteApp("frontend", "../Frontend")
    .WithReference(webapi)
    .WithExternalHttpEndpoints();

builder.Build().Run();
