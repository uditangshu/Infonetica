using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;

namespace ConfigurableWorkflowEngine.Services;

public interface IWorkflowService
{
    // Workflow Definitions
    Task<(bool Success, WorkflowDefinition? Definition, List<ValidationError> Errors)> CreateDefinitionAsync(WorkflowDefinition definition);
    Task<WorkflowDefinition?> GetDefinitionAsync(string id);
    Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync();

    // Workflow Instances
    Task<(bool Success, WorkflowInstance? Instance, List<ValidationError> Errors)> CreateInstanceAsync(CreateInstanceRequest request);
    Task<WorkflowInstance?> GetInstanceAsync(string id);
    Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync();
    Task<IEnumerable<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId);

    // Workflow Actions
    Task<(bool Success, WorkflowInstance? Instance, List<ValidationError> Errors)> ExecuteActionAsync(string instanceId, ExecuteActionRequest request);
    Task<List<WorkflowAction>> GetAvailableActionsAsync(string instanceId);
} 