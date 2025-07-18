using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowInstances;

public static class GetAll
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync()
        {
            var instances = await workflowService.GetAllInstancesAsync();
            return Results.Ok(ApiResponse<IEnumerable<WorkflowInstance>>.Ok(instances));
        }
    }
} 