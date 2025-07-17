using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Requests;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Core.Interfaces;

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

    // Workflow Definitions
    public async Task<(bool Success, WorkflowDefinition? Definition, List<ValidationError> Errors)> CreateDefinitionAsync(WorkflowDefinition definition)
    {
        var (isValid, errors) = await _validationService.ValidateDefinitionAsync(definition);
        if (!isValid)
        {
            return (false, null, errors);
        }

        var createdDefinition = await _repository.CreateDefinitionAsync(definition);
        return (true, createdDefinition, new List<ValidationError>());
    }

    public async Task<WorkflowDefinition?> GetDefinitionAsync(string id)
    {
        return await _repository.GetDefinitionAsync(id);
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetAllDefinitionsAsync()
    {
        return await _repository.GetAllDefinitionsAsync();
    }

    // Workflow Instances
    public async Task<(bool Success, WorkflowInstance? Instance, List<ValidationError> Errors)> CreateInstanceAsync(CreateInstanceRequest request)
    {
        var (isValid, errors) = await _validationService.ValidateInstanceCreationAsync(request.DefinitionId);
        if (!isValid)
        {
            return (false, null, errors);
        }

        var definition = await _repository.GetDefinitionAsync(request.DefinitionId);
        if (definition == null)
        {
            return (false, null, new List<ValidationError>
            {
                new ValidationError { Field = "definitionId", Message = $"Definition '{request.DefinitionId}' not found" }
            });
        }

        var initialState = definition.States.First(s => s.IsInitial);
        var instance = new WorkflowInstance
        {
            DefinitionId = request.DefinitionId,
            CurrentState = initialState.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdInstance = await _repository.CreateInstanceAsync(instance);
        return (true, createdInstance, new List<ValidationError>());
    }

    public async Task<WorkflowInstance?> GetInstanceAsync(string id)
    {
        return await _repository.GetInstanceAsync(id);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetAllInstancesAsync()
    {
        return await _repository.GetAllInstancesAsync();
    }

    public async Task<IEnumerable<WorkflowInstance>> GetInstancesByDefinitionAsync(string definitionId)
    {
        return await _repository.GetInstancesByDefinitionAsync(definitionId);
    }

    // Workflow Actions
    public async Task<(bool Success, WorkflowInstance? Instance, List<ValidationError> Errors)> ExecuteActionAsync(string instanceId, ExecuteActionRequest request)
    {
        var (isValid, errors) = await _validationService.ValidateActionExecutionAsync(instanceId, request.ActionId);
        if (!isValid)
        {
            return (false, null, errors);
        }

        var instance = await _repository.GetInstanceAsync(instanceId);
        if (instance == null)
        {
            return (false, null, new List<ValidationError>
            {
                new ValidationError { Field = "instanceId", Message = $"Instance '{instanceId}' not found" }
            });
        }

        var definition = await _repository.GetDefinitionAsync(instance.DefinitionId);
        if (definition == null)
        {
            return (false, null, new List<ValidationError>
            {
                new ValidationError { Field = "definition", Message = $"Definition '{instance.DefinitionId}' not found" }
            });
        }

        var action = definition.Actions.First(a => a.Id == request.ActionId);
        var currentState = definition.States.First(s => s.Id == instance.CurrentState);
        var targetState = definition.States.First(s => s.Id == action.ToState);

        // Create history entry
        var historyEntry = new HistoryEntry
        {
            ActionId = action.Id,
            ActionName = action.Name,
            FromState = instance.CurrentState,
            ToState = action.ToState,
            Timestamp = DateTime.UtcNow,
            ExecutedBy = request.ExecutedBy
        };

        // Update instance
        instance.CurrentState = action.ToState;
        instance.UpdatedAt = DateTime.UtcNow;
        instance.History.Add(historyEntry);

        // Update status if moved to final state
        if (targetState.IsFinal)
        {
            instance.Status = WorkflowStatus.Completed;
        }

        var updatedInstance = await _repository.UpdateInstanceAsync(instanceId, instance);
        return (true, updatedInstance, new List<ValidationError>());
    }

    public async Task<List<WorkflowAction>> GetAvailableActionsAsync(string instanceId)
    {
        var instance = await _repository.GetInstanceAsync(instanceId);
        if (instance == null)
        {
            return new List<WorkflowAction>();
        }

        var definition = await _repository.GetDefinitionAsync(instance.DefinitionId);
        if (definition == null)
        {
            return new List<WorkflowAction>();
        }

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState == null || currentState.IsFinal)
        {
            return new List<WorkflowAction>();
        }

        return definition.Actions
            .Where(a => a.Enabled && a.FromStates.Contains(instance.CurrentState))
            .ToList();
    }
} 