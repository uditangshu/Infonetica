using ConfigurableWorkflowEngine.Models.Entities;

namespace ConfigurableWorkflowEngine.Core.Interfaces;

public interface IWorkflowRepository
{
    // Workflow Definitions
    Task<WorkflowDefinition?> GetDefinitionAsync(string id);
    Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync();
    Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition);
    Task<WorkflowDefinition?> UpdateDefinitionAsync(string id, WorkflowDefinition definition);
    Task<bool> DeleteDefinitionAsync(string id);

    // Workflow Instances
    Task<WorkflowInstance?> GetInstanceAsync(string id);
    Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync();
    Task<IEnumerable<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId);
    Task<WorkflowInstance> CreateInstanceAsync(WorkflowInstance instance);
    Task<WorkflowInstance?> UpdateInstanceAsync(string id, WorkflowInstance instance);
    Task<bool> DeleteInstanceAsync(string id);

    // Persistence
    Task SaveToFileAsync();
    Task LoadFromFileAsync();
} 