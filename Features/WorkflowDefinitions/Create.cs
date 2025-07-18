using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowDefinitions;

public static class Create
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync(WorkflowDefinition definition)
        {
            var (success, createdDefinition, errors) = await workflowService.CreateDefinitionAsync(definition);

            if (success)
            {
                // Use CreatedAtRoute for safe URL generation
                return Results.CreatedAtRoute("GetWorkflowDefinition",
                    new { id = createdDefinition!.Id },
                    ApiResponse<WorkflowDefinition>.Ok(createdDefinition));
            }

            return Results.BadRequest(ApiResponse<WorkflowDefinition>.Fail(
                $"Validation failed: {string.Join(", ", errors.Select(e => e.Message))}"));
        }
    }
} 