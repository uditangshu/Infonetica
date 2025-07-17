using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowActions;

public static class GetAvailable
{
    public class Handler
    {
        private readonly IWorkflowService _workflowService;

        public Handler(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        public async Task<IResult> HandleAsync(string instanceId)
        {
            var actions = await _workflowService.GetAvailableActionsAsync(instanceId);
            return Results.Ok(ApiResponse<List<WorkflowAction>>.Ok(actions));
        }
    }
} 