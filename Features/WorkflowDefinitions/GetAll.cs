using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Services;

namespace ConfigurableWorkflowEngine.Features.WorkflowDefinitions;

public static class GetAll
{
    public class Handler
    {
        private readonly IWorkflowService _workflowService;

        public Handler(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        public async Task<IResult> HandleAsync()
        {
            var definitions = await _workflowService.GetAllDefinitionsAsync();
            return Results.Ok(ApiResponse<IEnumerable<WorkflowDefinition>>.Ok(definitions));
        }
    }
} 