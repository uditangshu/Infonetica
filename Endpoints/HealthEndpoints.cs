namespace ConfigurableWorkflowEngine.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
       app.MapGet("/ping", () => Results.Ok(new {
        status = "OK",
        message = "Pong",
        timestamp = DateTime.UtcNow,
        version = "1.0.0",
        environment = Environment.MachineName,
        uptime = TimeSpan.FromMilliseconds(Environment.TickCount)
       }))
       .WithTags("Ping")
       .WithOpenApi();
    }
} 