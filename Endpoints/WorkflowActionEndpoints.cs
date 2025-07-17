using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Endpoints;

public static class WorkflowActionEndpoints
{
    public static void MapWorkflowActionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workflows/instances/{instanceId}/actions")
            .WithTags("Workflow Actions");

        // Execute an action on a workflow instance
        group.MapPost("/", ExecuteWorkflowAction)
            .WithName("ExecuteWorkflowAction")
            .WithOpenApi();

        // Get available actions for a workflow instance
        group.MapGet("/", GetAvailableActions)
            .WithName("GetAvailableActions")
            .WithOpenApi();
    }

    private static async Task<IResult> ExecuteWorkflowAction(
        string instanceId, 
        ExecuteActionRequest request, 
        IWorkflowService workflowService)
    {
        var (success, updatedInstance, errors) = await workflowService.ExecuteActionAsync(instanceId, request);
        
        if (success)
        {
            return Results.Ok(ApiResponse<WorkflowInstance>.Ok(updatedInstance!));
        }
        
        return Results.BadRequest(ApiResponse<WorkflowInstance>.Fail(
            $"Action execution failed: {string.Join(", ", errors.Select(e => e.Message))}"));
    }

    private static async Task<IResult> GetAvailableActions(
        string instanceId, 
        IWorkflowService workflowService)
    {
        var actions = await workflowService.GetAvailableActionsAsync(instanceId);
        return Results.Ok(ApiResponse<List<WorkflowAction>>.Ok(actions));
    }
} 