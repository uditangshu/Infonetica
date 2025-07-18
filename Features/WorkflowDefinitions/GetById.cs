using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowDefinitions;

public static class GetById
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync(string id)
        {
            var definition = await workflowService.GetDefinitionAsync(id);

            if (definition == null)
            {
                return Results.NotFound(ApiResponse<WorkflowDefinition>.Fail(
                    $"Workflow definition '{id}' not found"));
            }

            return Results.Ok(ApiResponse<WorkflowDefinition>.Ok(definition));
        }
    }
} 