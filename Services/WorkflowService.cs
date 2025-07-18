using ConfigurableWorkflowEngine.Core.Interfaces;
using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;

namespace ConfigurableWorkflowEngine.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowRepository _repository;
    private readonly IWorkflowValidationService _validationService;

    public WorkflowService(IWorkflowRepository repository, IWorkflowValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
    }

    public async Task<(bool Success, WorkflowDefinition? Definition, List<ValidationError> Errors)> CreateDefinitionAsync(WorkflowDefinition definition)
    {
        var errors = _validationService.ValidateDefinition(definition);
        if (errors.Any()) return (false, null, errors);

        await _repository.CreateDefinitionAsync(definition);
        return (true, definition, new());
    }

    public async Task<WorkflowDefinition?> GetDefinitionAsync(string id) =>
        await _repository.GetDefinitionAsync(id);

    public async Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync() =>
        await _repository.GetAllDefinitionsAsync();

    public async Task<(bool Success, WorkflowDefinition? Definition, List<ValidationError> Errors)> UpdateDefinitionStatesAsync(string definitionId, UpdateStatesRequest request)
    {
        var definition = await _repository.GetDefinitionAsync(definitionId);
        if (definition == null) return (false, null, new() { new() { Message = $"Definition '{definitionId}' not found" } });

        var updated = new WorkflowDefinition
        {
            Id = definition.Id,
            Name = definition.Name,
            Description = definition.Description,
            States = new(definition.States),
            Actions = new(definition.Actions)
        };

        var errors = new List<ValidationError>();

        // Add states
        if (request.Add != null)
        {
            foreach (var state in request.Add)
            {
                if (updated.States.Any(s => s.Id == state.Id))
                    errors.Add(new() { Message = $"State '{state.Id}' already exists" });
                else
                    updated.States.Add(state);
            }
        }

        // Update states
        if (request.Update != null)
        {
            foreach (var stateUpdate in request.Update)
            {
                var existing = updated.States.FirstOrDefault(s => s.Id == stateUpdate.Id);
                if (existing == null)
                    errors.Add(new() { Message = $"State '{stateUpdate.Id}' not found" });
                else
                {
                    existing.Name = stateUpdate.Name;
                    existing.Description = stateUpdate.Description;
                    existing.IsInitial = stateUpdate.IsInitial;
                    existing.IsFinal = stateUpdate.IsFinal;
                    existing.Enabled = stateUpdate.Enabled;
                }
            }
        }

        // Remove states
        if (request.Remove != null)
        {
            foreach (var stateId in request.Remove)
            {
                var state = updated.States.FirstOrDefault(s => s.Id == stateId);
                if (state == null)
                    errors.Add(new() { Message = $"State '{stateId}' not found" });
                else
                {
                    var referencingActions = updated.Actions.Where(a => a.FromStates.Contains(stateId) || a.ToState == stateId);
                    if (referencingActions.Any())
                        errors.Add(new() { Message = $"Cannot remove state '{stateId}' - referenced by actions: {string.Join(", ", referencingActions.Select(a => a.Id))}" });
                    else
                        updated.States.Remove(state);
                }
            }
        }

        if (errors.Any()) return (false, null, errors);

        var validationErrors = _validationService.ValidateDefinition(updated);
        if (validationErrors.Any()) return (false, null, validationErrors);

        await _repository.UpdateDefinitionAsync(updated);
        return (true, updated, new());
    }

    public async Task<(bool Success, WorkflowDefinition? Definition, List<ValidationError> Errors)> UpdateDefinitionActionsAsync(string definitionId, UpdateActionsRequest request)
    {
        var definition = await _repository.GetDefinitionAsync(definitionId);
        if (definition == null) return (false, null, new() { new() { Message = $"Definition '{definitionId}' not found" } });

        var updated = new WorkflowDefinition
        {
            Id = definition.Id,
            Name = definition.Name,
            Description = definition.Description,
            States = new(definition.States),
            Actions = new(definition.Actions)
        };

        var errors = new List<ValidationError>();

        // Add actions
        if (request.Add != null)
        {
            foreach (var action in request.Add)
            {
                if (updated.Actions.Any(a => a.Id == action.Id))
                    errors.Add(new() { Message = $"Action '{action.Id}' already exists" });
                else
                    updated.Actions.Add(action);
            }
        }

        // Update actions
        if (request.Update != null)
        {
            foreach (var actionUpdate in request.Update)
            {
                var existing = updated.Actions.FirstOrDefault(a => a.Id == actionUpdate.Id);
                if (existing == null)
                    errors.Add(new() { Message = $"Action '{actionUpdate.Id}' not found" });
                else
                {
                    existing.Name = actionUpdate.Name;
                    existing.Description = actionUpdate.Description;
                    existing.Enabled = actionUpdate.Enabled;
                    existing.FromStates = actionUpdate.FromStates;
                    existing.ToState = actionUpdate.ToState;
                }
            }
        }

        // Remove actions
        if (request.Remove != null)
        {
            foreach (var actionId in request.Remove)
            {
                var action = updated.Actions.FirstOrDefault(a => a.Id == actionId);
                if (action == null)
                    errors.Add(new() { Message = $"Action '{actionId}' not found" });
                else
                    updated.Actions.Remove(action);
            }
        }

        if (errors.Any()) return (false, null, errors);

        var validationErrors = _validationService.ValidateDefinition(updated);
        if (validationErrors.Any()) return (false, null, validationErrors);

        await _repository.UpdateDefinitionAsync(updated);
        return (true, updated, new());
    }

    public async Task<(bool Success, WorkflowInstance? Instance, List<ValidationError> Errors)> CreateInstanceAsync(CreateInstanceRequest request)
    {
        var definition = await _repository.GetDefinitionAsync(request.DefinitionId);
        if (definition == null) return (false, null, new() { new() { Message = $"Definition '{request.DefinitionId}' not found" } });

        var initialState = definition.States.FirstOrDefault(s => s.IsInitial);
        if (initialState == null) return (false, null, new() { new() { Message = "No initial state found" } });

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid().ToString(),
            DefinitionId = request.DefinitionId,
            Name = request.Name,
            Description = request.Description,
            CurrentState = initialState.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            History = new() { new() { StateId = initialState.Id, Timestamp = DateTime.UtcNow, ExecutedBy = "system", Comment = "Instance created" } }
        };

        await _repository.CreateInstanceAsync(instance);
        return (true, instance, new());
    }

    public async Task<WorkflowInstance?> GetInstanceAsync(string id) =>
        await _repository.GetInstanceAsync(id);

    public async Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync() =>
        await _repository.GetAllInstancesAsync();

    public async Task<IEnumerable<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId) =>
        await _repository.GetInstancesByDefinitionAsync(definitionId);

    public async Task<(bool Success, WorkflowInstance? Instance, List<ValidationError> Errors)> ExecuteActionAsync(string instanceId, ExecuteActionRequest request)
    {
        var instance = await _repository.GetInstanceAsync(instanceId);
        if (instance == null) return (false, null, new() { new() { Message = $"Instance '{instanceId}' not found" } });

        var definition = await _repository.GetDefinitionAsync(instance.DefinitionId);
        if (definition == null) return (false, null, new() { new() { Message = $"Definition '{instance.DefinitionId}' not found" } });

        var errors = _validationService.ValidateAction(definition, instance, request.ActionId);
        if (errors.Any()) return (false, null, errors);

        var action = definition.Actions.First(a => a.Id == request.ActionId);
        instance.CurrentState = action.ToState;
        instance.UpdatedAt = DateTime.UtcNow;
        instance.History.Add(new() { StateId = action.ToState, Timestamp = DateTime.UtcNow, ActionId = request.ActionId, ExecutedBy = request.ExecutedBy ?? "unknown", Comment = request.Comment });

        await _repository.UpdateInstanceAsync(instance);
        return (true, instance, new());
    }

    public async Task<List<WorkflowAction>> GetAvailableActionsAsync(string instanceId)
    {
        var instance = await _repository.GetInstanceAsync(instanceId);
        if (instance == null) return new();

        var definition = await _repository.GetDefinitionAsync(instance.DefinitionId);
        if (definition == null) return new();

        return definition.Actions.Where(a => a.Enabled && a.FromStates.Contains(instance.CurrentState)).ToList();
    }
} 