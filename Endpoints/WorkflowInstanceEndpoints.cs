using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Endpoints;

public static class WorkflowInstanceEndpoints
{
    public static void MapWorkflowInstanceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workflows/instances")
            .WithTags("Workflow Instances");

        // Create a new workflow instance
        group.MapPost("/", CreateWorkflowInstance)
            .WithName("CreateWorkflowInstance")
            .WithOpenApi();

        // Get a specific workflow instance
        group.MapGet("/{id}", GetWorkflowInstance)
            .WithName("GetWorkflowInstance")
            .WithOpenApi();

        // Get all workflow instances
        group.MapGet("/", GetAllWorkflowInstances)
            .WithName("GetAllWorkflowInstances")
            .WithOpenApi();

        // Get instances by definition
        app.MapGet("/api/workflows/definitions/{definitionId}/instances", GetInstancesByDefinition)
            .WithName("GetInstancesByDefinition")
            .WithTags("Workflow Instances")
            .WithOpenApi();
    }

    private static async Task<IResult> CreateWorkflowInstance(
        CreateInstanceRequest request, 
        IWorkflowService workflowService)
    {
        var (success, createdInstance, errors) = await workflowService.CreateInstanceAsync(request);
        
        if (success)
        {
            return Results.Created($"/api/workflows/instances/{createdInstance!.Id}", 
                ApiResponse<WorkflowInstance>.Ok(createdInstance));
        }
        
        return Results.BadRequest(ApiResponse<WorkflowInstance>.Fail(
            $"Validation failed: {string.Join(", ", errors.Select(e => e.Message))}"));
    }

    private static async Task<IResult> GetWorkflowInstance(
        string id, 
        IWorkflowService workflowService)
    {
        var instance = await workflowService.GetInstanceAsync(id);
        
        if (instance == null)
        {
            return Results.NotFound(ApiResponse<WorkflowInstance>.Fail(
                $"Workflow instance '{id}' not found"));
        }
        
        return Results.Ok(ApiResponse<WorkflowInstance>.Ok(instance));
    }

    private static async Task<IResult> GetAllWorkflowInstances(
        IWorkflowService workflowService)
    {
        var instances = await workflowService.GetAllInstancesAsync();
        return Results.Ok(ApiResponse<IEnumerable<WorkflowInstance>>.Ok(instances));
    }

    private static async Task<IResult> GetInstancesByDefinition(
        string definitionId, 
        IWorkflowService workflowService)
    {
        var instances = await workflowService.GetInstancesByDefinitionAsync(definitionId);
        return Results.Ok(ApiResponse<IEnumerable<WorkflowInstance>>.Ok(instances));
    }
} 