using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowActions;

public static class Execute
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync(string instanceId, ExecuteActionRequest request)
        {
            var (success, updatedInstance, errors) = await workflowService.ExecuteActionAsync(instanceId, request);

            if (success)
            {
                return Results.Ok(ApiResponse<WorkflowInstance>.Ok(updatedInstance!));
            }

            return Results.BadRequest(ApiResponse<WorkflowInstance>.Fail(
                $"Action execution failed: {string.Join(", ", errors.Select(e => e.Message))}"));
        }
    }
} 