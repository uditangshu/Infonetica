using System.Text.Json;
using ConfigurableWorkflowEngine.Core.Interfaces;
using ConfigurableWorkflowEngine.Models.Entities;

namespace ConfigurableWorkflowEngine.Infrastructure.Repositories;

public class InMemoryWorkflowRepository : IWorkflowRepository
{
    private static readonly Dictionary<string, WorkflowDefinition> _definitions = new();
    private static readonly Dictionary<string, WorkflowInstance> _instances = new();
    private static readonly object _lock = new();

    private const string DefinitionsFile = "workflow-definitions.json";
    private const string InstancesFile = "workflow-instances.json";

    public InMemoryWorkflowRepository()
    {
        LoadDefinitions();
        LoadInstances();
    }

    // Workflow Definitions
    public Task CreateDefinitionAsync(WorkflowDefinition definition)
    {
        lock (_lock)
        {
            _definitions[definition.Id] = definition;
        }
        return SaveDefinitionsAsync();
    }

    public Task<WorkflowDefinition?> GetDefinitionAsync(string id)
    {
        lock (_lock)
        {
            _definitions.TryGetValue(id, out var definition);
            return Task.FromResult(definition);
        }
    }

    public Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_definitions.Values.AsEnumerable());
        }
    }

    public Task UpdateDefinitionAsync(WorkflowDefinition definition)
    {
        lock (_lock)
        {
            _definitions[definition.Id] = definition;
        }
        return SaveDefinitionsAsync();
    }

    // Workflow Instances
    public Task CreateInstanceAsync(WorkflowInstance instance)
    {
        lock (_lock)
        {
            _instances[instance.Id] = instance;
        }
        return SaveInstancesAsync();
    }

    public Task<WorkflowInstance?> GetInstanceAsync(string id)
    {
        lock (_lock)
        {
            _instances.TryGetValue(id, out var instance);
            return Task.FromResult(instance);
        }
    }

    public Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_instances.Values.AsEnumerable());
        }
    }

    public Task<IEnumerable<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId)
    {
        lock (_lock)
        {
            var instances = _instances.Values.Where(i => i.DefinitionId == definitionId);
            return Task.FromResult(instances);
        }
    }

    public Task UpdateInstanceAsync(WorkflowInstance instance)
    {
        lock (_lock)
        {
            _instances[instance.Id] = instance;
        }
        return SaveInstancesAsync();
    }

    // Private helper methods
    private void LoadDefinitions()
    {
        if (File.Exists(DefinitionsFile))
        {
            var json = File.ReadAllText(DefinitionsFile);
            var definitions = JsonSerializer.Deserialize<Dictionary<string, WorkflowDefinition>>(json);
            if (definitions != null)
            {
                foreach (var kvp in definitions)
                {
                    _definitions[kvp.Key] = kvp.Value;
                }
            }
        }
    }

    private void LoadInstances()
    {
        if (File.Exists(InstancesFile))
        {
            var json = File.ReadAllText(InstancesFile);
            var instances = JsonSerializer.Deserialize<Dictionary<string, WorkflowInstance>>(json);
            if (instances != null)
            {
                foreach (var kvp in instances)
                {
                    _instances[kvp.Key] = kvp.Value;
                }
            }
        }
    }

    private async Task SaveDefinitionsAsync()
    {
        var json = JsonSerializer.Serialize(_definitions, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(DefinitionsFile, json);
    }

    private async Task SaveInstancesAsync()
    {
        var json = JsonSerializer.Serialize(_instances, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(InstancesFile, json);
    }
} 