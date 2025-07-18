using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowActions;

public static class GetAvailable
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync(string instanceId)
        {
            var actions = await workflowService.GetAvailableActionsAsync(instanceId);
            return Results.Ok(ApiResponse<IEnumerable<WorkflowAction>>.Ok(actions));
        }
    }
} 