namespace ConfigurableWorkflowEngine.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        // Health check endpoint
        app.MapGet("/health", () => Results.Ok(new { 
            Status = "Healthy", 
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.MachineName
        }))
        .WithName("HealthCheck")
        .WithTags("Health")
        .WithOpenApi();

        // Root redirect to swagger
        app.MapGet("/", () => Results.Redirect("/swagger"))
        .WithName("Root")
        .ExcludeFromDescription();
    }
} 