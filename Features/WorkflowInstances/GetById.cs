using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowInstances;

public static class GetById
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync(string id)
        {
            var instance = await workflowService.GetInstanceAsync(id);

            if (instance == null)
            {
                return Results.NotFound(ApiResponse<WorkflowInstance>.Fail(
                    $"Workflow instance '{id}' not found"));
            }

            return Results.Ok(ApiResponse<WorkflowInstance>.Ok(instance));
        }
    }
} 