using Microsoft.AspNetCore.WebSockets;
using WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddWebSockets(options => { });
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();
app.UseWebSockets();

app.MapGet("/ping", () => "pong");
app.MapCustomerManagement();
app.MapWebSocketEndpoints();

app.Run();
