using ConfigurableWorkflowEngine.Features.WorkflowDefinitions;
using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurableWorkflowEngine.Endpoints;

public static class WorkflowDefinitionEndpoints
{
    public static void MapWorkflowDefinitionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workflows/definitions")
            .WithTags("Workflow Definitions");

        // Create a new workflow definition
        group.MapPost("/", async (WorkflowDefinition definition, [FromServices] Create.Handler handler) => 
            await handler.HandleAsync(definition))
            .WithName("CreateWorkflowDefinition")
            .WithOpenApi();

        // Get a specific workflow definition
        group.MapGet("/{id}", async (string id, [FromServices] GetById.Handler handler) => 
            await handler.HandleAsync(id))
            .WithName("GetWorkflowDefinition")
            .WithOpenApi();

        // Get all workflow definitions
        group.MapGet("/", async ([FromServices] GetAll.Handler handler) => 
            await handler.HandleAsync())
            .WithName("GetAllWorkflowDefinitions")
            .WithOpenApi();

        // Update states in a workflow definition (incremental)
        group.MapPatch("/{id}/states", async (string id, UpdateStatesRequest request, [FromServices] UpdateStates.Handler handler) => 
            await handler.HandleAsync(id, request))
            .WithName("UpdateWorkflowDefinitionStates")
            .WithOpenApi();

        // Update actions in a workflow definition (incremental)
        group.MapPatch("/{id}/actions", async (string id, UpdateActionsRequest request, [FromServices] UpdateActions.Handler handler) => 
            await handler.HandleAsync(id, request))
            .WithName("UpdateWorkflowDefinitionActions")
            .WithOpenApi();
    }
} 