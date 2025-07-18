using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowDefinitions;

public static class GetAll
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync()
        {
            var definitions = await workflowService.GetAllDefinitionsAsync();
            return Results.Ok(ApiResponse<IEnumerable<WorkflowDefinition>>.Ok(definitions));
        }
    }
} 