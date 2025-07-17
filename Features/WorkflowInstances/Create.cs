using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowInstances;

public static class Create
{
    public class Handler
    {
        private readonly IWorkflowService _workflowService;

        public Handler(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        public async Task<IResult> HandleAsync(CreateInstanceRequest request)
        {
            var (success, createdInstance, errors) = await _workflowService.CreateInstanceAsync(request);

            if (success)
            {
                return Results.CreatedAtRoute("GetWorkflowInstance",
                    new { id = createdInstance!.Id },
                    ApiResponse<WorkflowInstance>.Ok(createdInstance));
            }

            return Results.BadRequest(ApiResponse<WorkflowInstance>.Fail(
                $"Validation failed: {string.Join(", ", errors.Select(e => e.Message))}"));
        }
    }
} 