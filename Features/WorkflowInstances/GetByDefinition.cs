using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowInstances;

public static class GetByDefinition
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync(string definitionId)
        {
            var instances = await workflowService.GetInstancesByDefinitionAsync(definitionId);
            return Results.Ok(ApiResponse<IEnumerable<WorkflowInstance>>.Ok(instances));
        }
    }
} 