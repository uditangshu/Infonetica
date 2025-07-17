using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Core.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ConfigurableWorkflowEngine.Infrastructure.Repositories;

public class InMemoryWorkflowRepository : IWorkflowRepository
{
    private readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new();
    private readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();
    private readonly string _definitionsFile = "workflow-definitions.json";
    private readonly string _instancesFile = "workflow-instances.json";
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public InMemoryWorkflowRepository()
    {
        // Load data from files on startup
        Task.Run(async () => await LoadFromFileAsync()).Wait();
    }

    // Workflow Definitions
    public Task<WorkflowDefinition?> GetDefinitionAsync(string id)
    {
        _definitions.TryGetValue(id, out var definition);
        return Task.FromResult(definition);
    }

    public Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync()
    {
        return Task.FromResult(_definitions.Values.AsEnumerable());
    }

    public async Task<WorkflowDefinition> CreateDefinitionAsync(WorkflowDefinition definition)
    {
        _definitions[definition.Id] = definition;
        await SaveToFileAsync();
        return definition;
    }

    public async Task<WorkflowDefinition?> UpdateDefinitionAsync(string id, WorkflowDefinition definition)
    {
        if (_definitions.ContainsKey(id))
        {
            _definitions[id] = definition;
            await SaveToFileAsync();
            return definition;
        }
        return null;
    }

    public async Task<bool> DeleteDefinitionAsync(string id)
    {
        var removed = _definitions.TryRemove(id, out _);
        if (removed)
        {
            await SaveToFileAsync();
        }
        return removed;
    }

    // Workflow Instances
    public Task<WorkflowInstance?> GetInstanceAsync(string id)
    {
        _instances.TryGetValue(id, out var instance);
        return Task.FromResult(instance);
    }

    public Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync()
    {
        return Task.FromResult(_instances.Values.AsEnumerable());
    }

    public Task<IEnumerable<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId)
    {
        var instances = _instances.Values.Where(i => i.DefinitionId == definitionId);
        return Task.FromResult(instances);
    }

    public async Task<WorkflowInstance> CreateInstanceAsync(WorkflowInstance instance)
    {
        _instances[instance.Id] = instance;
        await SaveToFileAsync();
        return instance;
    }

    public async Task<WorkflowInstance?> UpdateInstanceAsync(string id, WorkflowInstance instance)
    {
        if (_instances.ContainsKey(id))
        {
            _instances[id] = instance;
            await SaveToFileAsync();
            return instance;
        }
        return null;
    }

    public async Task<bool> DeleteInstanceAsync(string id)
    {
        var removed = _instances.TryRemove(id, out _);
        if (removed)
        {
            await SaveToFileAsync();
        }
        return removed;
    }

    // Persistence
    public async Task SaveToFileAsync()
    {
        try
        {
            // Save definitions
            var definitionsJson = JsonSerializer.Serialize(_definitions.Values, _jsonOptions);
            await File.WriteAllTextAsync(_definitionsFile, definitionsJson);

            // Save instances
            var instancesJson = JsonSerializer.Serialize(_instances.Values, _jsonOptions);
            await File.WriteAllTextAsync(_instancesFile, instancesJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to files: {ex.Message}");
        }
    }

    public async Task LoadFromFileAsync()
    {
        try
        {
            // Load definitions
            if (File.Exists(_definitionsFile))
            {
                var definitionsJson = await File.ReadAllTextAsync(_definitionsFile);
                var definitions = JsonSerializer.Deserialize<WorkflowDefinition[]>(definitionsJson);
                if (definitions != null)
                {
                    _definitions.Clear();
                    foreach (var definition in definitions)
                    {
                        _definitions[definition.Id] = definition;
                    }
                }
            }

            // Load instances
            if (File.Exists(_instancesFile))
            {
                var instancesJson = await File.ReadAllTextAsync(_instancesFile);
                var instances = JsonSerializer.Deserialize<WorkflowInstance[]>(instancesJson);
                if (instances != null)
                {
                    _instances.Clear();
                    foreach (var instance in instances)
                    {
                        _instances[instance.Id] = instance;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from files: {ex.Message}");
        }
    }
} 