using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowInstances;

public static class GetByDefinition
{
    public class Handler
    {
        private readonly IWorkflowService _workflowService;

        public Handler(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        public async Task<IResult> HandleAsync(string definitionId)
        {
            var instances = await _workflowService.GetInstancesByDefinitionAsync(definitionId);
            return Results.Ok(ApiResponse<IEnumerable<WorkflowInstance>>.Ok(instances));
        }
    }
} 