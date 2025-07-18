using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;

namespace ConfigurableWorkflowEngine.Services;

public interface IWorkflowService
{
    // Definition operations
    Task<(bool Success, WorkflowDefinition? Definition, List<ValidationError> Errors)> CreateDefinitionAsync(WorkflowDefinition definition);
    Task<WorkflowDefinition?> GetDefinitionAsync(string id);
    Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync();
    
    // New incremental update operations
    Task<(bool Success, WorkflowDefinition? Definition, List<ValidationError> Errors)> UpdateDefinitionStatesAsync(string definitionId, UpdateStatesRequest request);
    Task<(bool Success, WorkflowDefinition? Definition, List<ValidationError> Errors)> UpdateDefinitionActionsAsync(string definitionId, UpdateActionsRequest request);

    // Instance operations
    Task<(bool Success, WorkflowInstance? Instance, List<ValidationError> Errors)> CreateInstanceAsync(CreateInstanceRequest request);
    Task<WorkflowInstance?> GetInstanceAsync(string id);
    Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync();
    Task<IEnumerable<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId);

    // Action operations
    Task<(bool Success, WorkflowInstance? Instance, List<ValidationError> Errors)> ExecuteActionAsync(string instanceId, ExecuteActionRequest request);
    Task<List<WorkflowAction>> GetAvailableActionsAsync(string instanceId);
} 