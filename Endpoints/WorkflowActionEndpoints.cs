using ConfigurableWorkflowEngine.Features.WorkflowActions;
using ConfigurableWorkflowEngine.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurableWorkflowEngine.Endpoints;

public static class WorkflowActionEndpoints
{
    public static void MapWorkflowActionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workflows/instances/{instanceId}/actions")
            .WithTags("Workflow Actions");

        // Execute an action on a workflow instance
        group.MapPost("/", async (string instanceId, ExecuteActionRequest request, [FromServices] Execute.Handler handler) => 
            await handler.HandleAsync(instanceId, request))
            .WithName("ExecuteWorkflowAction")
            .WithOpenApi();

        // Get available actions for a workflow instance
        group.MapGet("/", async (string instanceId, [FromServices] GetAvailable.Handler handler) => 
            await handler.HandleAsync(instanceId))
            .WithName("GetAvailableActions")
            .WithOpenApi();
    }
} 