using ConfigurableWorkflowEngine.Models.Entities;

namespace ConfigurableWorkflowEngine.Core.Interfaces;

public interface IWorkflowRepository
{
    // Workflow Definitions
    Task CreateDefinitionAsync(WorkflowDefinition definition);
    Task<WorkflowDefinition?> GetDefinitionAsync(string id);
    Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync();
    Task UpdateDefinitionAsync(WorkflowDefinition definition);

    // Workflow Instances
    Task CreateInstanceAsync(WorkflowInstance instance);
    Task<WorkflowInstance?> GetInstanceAsync(string id);
    Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync();
    Task<IEnumerable<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId);
    Task UpdateInstanceAsync(WorkflowInstance instance);
} 