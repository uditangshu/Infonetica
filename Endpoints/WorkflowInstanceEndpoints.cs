using ConfigurableWorkflowEngine.Features.WorkflowInstances;
using ConfigurableWorkflowEngine.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurableWorkflowEngine.Endpoints;

public static class WorkflowInstanceEndpoints
{
    public static void MapWorkflowInstanceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workflows/instances")
            .WithTags("Workflow Instances");

        // Create a new workflow instance
        group.MapPost("/", async (CreateInstanceRequest request, [FromServices] Create.Handler handler) => 
            await handler.HandleAsync(request))
            .WithName("CreateWorkflowInstance")
            .WithOpenApi();

        // Get a specific workflow instance
        group.MapGet("/{id}", async (string id, [FromServices] GetById.Handler handler) => 
            await handler.HandleAsync(id))
            .WithName("GetWorkflowInstance")
            .WithOpenApi();

        // Get all workflow instances
        group.MapGet("/", async ([FromServices] GetAll.Handler handler) => 
            await handler.HandleAsync())
            .WithName("GetAllWorkflowInstances")
            .WithOpenApi();

        // Get instances by definition
        app.MapGet("/api/workflows/definitions/{definitionId}/instances", async (string definitionId, [FromServices] GetByDefinition.Handler handler) => 
            await handler.HandleAsync(definitionId))
            .WithName("GetInstancesByDefinition")
            .WithTags("Workflow Instances")
            .WithOpenApi();
    }
} 