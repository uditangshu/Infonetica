using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowDefinitions;

public static class UpdateActions
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync(string id, UpdateActionsRequest request)
        {
            var (success, updatedDefinition, errors) = await workflowService.UpdateDefinitionActionsAsync(id, request);

            if (success)
            {
                return Results.Ok(ApiResponse<WorkflowDefinition>.Ok(updatedDefinition!));
            }

            return Results.BadRequest(ApiResponse<WorkflowDefinition>.Fail(
                $"Actions update failed: {string.Join(", ", errors.Select(e => e.Message))}"));
        }
    }
} 