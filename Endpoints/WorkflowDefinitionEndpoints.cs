using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Endpoints;

public static class WorkflowDefinitionEndpoints
{
    public static void MapWorkflowDefinitionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workflows/definitions")
            .WithTags("Workflow Definitions");

        // Create a new workflow definition
        group.MapPost("/", CreateWorkflowDefinition)
            .WithName("CreateWorkflowDefinition")
            .WithOpenApi();

        // Get a specific workflow definition
        group.MapGet("/{id}", GetWorkflowDefinition)
            .WithName("GetWorkflowDefinition")
            .WithOpenApi();

        // Get all workflow definitions
        group.MapGet("/", GetAllWorkflowDefinitions)
            .WithName("GetAllWorkflowDefinitions")
            .WithOpenApi();
    }

    private static async Task<IResult> CreateWorkflowDefinition(
        WorkflowDefinition definition, 
        IWorkflowService workflowService)
    {
        var (success, createdDefinition, errors) = await workflowService.CreateDefinitionAsync(definition);
        
        if (success)
        {
            return Results.Created($"/api/workflows/definitions/{createdDefinition!.Id}", 
                ApiResponse<WorkflowDefinition>.Ok(createdDefinition));
        }
        
        return Results.BadRequest(ApiResponse<WorkflowDefinition>.Fail(
            $"Validation failed: {string.Join(", ", errors.Select(e => e.Message))}"));
    }

    private static async Task<IResult> GetWorkflowDefinition(
        string id, 
        IWorkflowService workflowService)
    {
        var definition = await workflowService.GetDefinitionAsync(id);
        
        if (definition == null)
        {
            return Results.NotFound(ApiResponse<WorkflowDefinition>.Fail(
                $"Workflow definition '{id}' not found"));
        }
        
        return Results.Ok(ApiResponse<WorkflowDefinition>.Ok(definition));
    }

    private static async Task<IResult> GetAllWorkflowDefinitions(
        IWorkflowService workflowService)
    {
        var definitions = await workflowService.GetAllDefinitionsAsync();
        return Results.Ok(ApiResponse<IEnumerable<WorkflowDefinition>>.Ok(definitions));
    }
} 