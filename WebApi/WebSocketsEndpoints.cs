using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebApi;

static class WebSocketEndpointsExtensions
{
    extension(IEndpointRouteBuilder app)
    {
        public IEndpointRouteBuilder MapWebSocketEndpoints()
        {
            var api = app.MapGroup("/ws");

            api.Map("/", async (HttpContext context) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    // New in .NET 10: We can treat WebSockets like a Stream
                    using var stream = WebSocketStream.Create(webSocket, WebSocketMessageType.Text, true);
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    string? message;
                    while((message = await reader.ReadLineAsync()) is not null)
                    {
                        message = message.Trim();
                        await JsonSerializer.SerializeAsync(stream, new { echo = message });
                        if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            });

            return app;
        }
    }
}