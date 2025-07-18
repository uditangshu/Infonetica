using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowDefinitions;

public static class UpdateStates
{
    public class Handler(IWorkflowService workflowService)
    {
        public async Task<IResult> HandleAsync(string id, UpdateStatesRequest request)
        {
            var (success, updatedDefinition, errors) = await workflowService.UpdateDefinitionStatesAsync(id, request);

            if (success)
            {
                return Results.Ok(ApiResponse<WorkflowDefinition>.Ok(updatedDefinition!));
            }

            return Results.BadRequest(ApiResponse<WorkflowDefinition>.Fail(
                $"States update failed: {string.Join(", ", errors.Select(e => e.Message))}"));
        }
    }
} 