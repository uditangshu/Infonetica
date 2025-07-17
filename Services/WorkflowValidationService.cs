using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Core.Interfaces;

namespace ConfigurableWorkflowEngine.Services;

public class WorkflowValidationService : IWorkflowValidationService
{
    private readonly IWorkflowRepository _repository;

    public WorkflowValidationService(IWorkflowRepository repository)
    {
        _repository = repository;
    }

    public async Task<(bool IsValid, List<ValidationError> Errors)> ValidateDefinitionAsync(WorkflowDefinition definition)
    {
        var errors = new List<ValidationError>();

        // Check if definition already exists
        var existingDefinition = await _repository.GetDefinitionAsync(definition.Id);
        if (existingDefinition != null)
        {
            errors.Add(new ValidationError
            {
                Field = "id",
                Message = $"Workflow definition with ID '{definition.Id}' already exists"
            });
        }

        // Validate basic structure
        if (string.IsNullOrWhiteSpace(definition.Id))
        {
            errors.Add(new ValidationError
            {
                Field = "id",
                Message = "Workflow definition ID is required"
            });
        }

        if (definition.States == null || !definition.States.Any())
        {
            errors.Add(new ValidationError
            {
                Field = "states",
                Message = "At least one state is required"
            });
        }

        if (definition.Actions == null)
        {
            errors.Add(new ValidationError
            {
                Field = "actions",
                Message = "Actions collection is required"
            });
        }

        if (definition.States?.Any() == true)
        {
            // Check for duplicate state IDs
            var stateIds = definition.States.Select(s => s.Id).ToList();
            var duplicateStateIds = stateIds.GroupBy(id => id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var duplicateId in duplicateStateIds)
            {
                errors.Add(new ValidationError
                {
                    Field = "states",
                    Message = $"Duplicate state ID: {duplicateId}"
                });
            }

            // Check for exactly one initial state
            var initialStates = definition.States.Where(s => s.IsInitial).ToList();
            if (initialStates.Count == 0)
            {
                errors.Add(new ValidationError
                {
                    Field = "states",
                    Message = "Exactly one initial state is required"
                });
            }
            else if (initialStates.Count > 1)
            {
                errors.Add(new ValidationError
                {
                    Field = "states",
                    Message = $"Only one initial state is allowed, found {initialStates.Count}"
                });
            }

            // Validate individual states
            foreach (var state in definition.States)
            {
                if (string.IsNullOrWhiteSpace(state.Id))
                {
                    errors.Add(new ValidationError
                    {
                        Field = $"states[{state.Id}].id",
                        Message = "State ID is required"
                    });
                }

                if (string.IsNullOrWhiteSpace(state.Name))
                {
                    errors.Add(new ValidationError
                    {
                        Field = $"states[{state.Id}].name",
                        Message = "State name is required"
                    });
                }
            }
        }

        if (definition.Actions?.Any() == true)
        {
            // Check for duplicate action IDs
            var actionIds = definition.Actions.Select(a => a.Id).ToList();
            var duplicateActionIds = actionIds.GroupBy(id => id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var duplicateId in duplicateActionIds)
            {
                errors.Add(new ValidationError
                {
                    Field = "actions",
                    Message = $"Duplicate action ID: {duplicateId}"
                });
            }

            // Validate individual actions
            foreach (var action in definition.Actions)
            {
                if (string.IsNullOrWhiteSpace(action.Id))
                {
                    errors.Add(new ValidationError
                    {
                        Field = $"actions[{action.Id}].id",
                        Message = "Action ID is required"
                    });
                }

                if (string.IsNullOrWhiteSpace(action.Name))
                {
                    errors.Add(new ValidationError
                    {
                        Field = $"actions[{action.Id}].name",
                        Message = "Action name is required"
                    });
                }

                if (action.FromStates == null || !action.FromStates.Any())
                {
                    errors.Add(new ValidationError
                    {
                        Field = $"actions[{action.Id}].fromStates",
                        Message = "At least one source state is required"
                    });
                }

                if (string.IsNullOrWhiteSpace(action.ToState))
                {
                    errors.Add(new ValidationError
                    {
                        Field = $"actions[{action.Id}].toState",
                        Message = "Target state is required"
                    });
                }

                // Validate state references
                if (definition.States != null)
                {
                    var stateIds = definition.States.Select(s => s.Id).ToHashSet();

                    // Check fromStates references
                    if (action.FromStates != null)
                    {
                        foreach (var fromState in action.FromStates)
                        {
                            if (!stateIds.Contains(fromState))
                            {
                                errors.Add(new ValidationError
                                {
                                    Field = $"actions[{action.Id}].fromStates",
                                    Message = $"Referenced state '{fromState}' does not exist"
                                });
                            }
                        }
                    }

                    // Check toState reference
                    if (!string.IsNullOrWhiteSpace(action.ToState) && !stateIds.Contains(action.ToState))
                    {
                        errors.Add(new ValidationError
                        {
                            Field = $"actions[{action.Id}].toState",
                            Message = $"Referenced state '{action.ToState}' does not exist"
                        });
                    }
                }
            }
        }

        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<ValidationError> Errors)> ValidateInstanceCreationAsync(string definitionId)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(definitionId))
        {
            errors.Add(new ValidationError
            {
                Field = "definitionId",
                Message = "Definition ID is required"
            });
            return (false, errors);
        }

        var definition = await _repository.GetDefinitionAsync(definitionId);
        if (definition == null)
        {
            errors.Add(new ValidationError
            {
                Field = "definitionId",
                Message = $"Workflow definition '{definitionId}' not found"
            });
            return (false, errors);
        }

        // Validate definition has initial state
        var initialState = definition.States.FirstOrDefault(s => s.IsInitial);
        if (initialState == null)
        {
            errors.Add(new ValidationError
            {
                Field = "definition",
                Message = "Workflow definition must have an initial state"
            });
        }

        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<ValidationError> Errors)> ValidateActionExecutionAsync(string instanceId, string actionId)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(instanceId))
        {
            errors.Add(new ValidationError
            {
                Field = "instanceId",
                Message = "Instance ID is required"
            });
        }

        if (string.IsNullOrWhiteSpace(actionId))
        {
            errors.Add(new ValidationError
            {
                Field = "actionId",
                Message = "Action ID is required"
            });
        }

        if (errors.Any())
        {
            return (false, errors);
        }

        // Get instance
        var instance = await _repository.GetInstanceAsync(instanceId);
        if (instance == null)
        {
            errors.Add(new ValidationError
            {
                Field = "instanceId",
                Message = $"Workflow instance '{instanceId}' not found"
            });
            return (false, errors);
        }

        // Check if instance is in final state
        var definition = await _repository.GetDefinitionAsync(instance.DefinitionId);
        if (definition == null)
        {
            errors.Add(new ValidationError
            {
                Field = "definition",
                Message = $"Workflow definition '{instance.DefinitionId}' not found"
            });
            return (false, errors);
        }

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState == null)
        {
            errors.Add(new ValidationError
            {
                Field = "currentState",
                Message = $"Current state '{instance.CurrentState}' not found in definition"
            });
            return (false, errors);
        }

        if (currentState.IsFinal)
        {
            errors.Add(new ValidationError
            {
                Field = "currentState",
                Message = $"Cannot execute actions on final state '{currentState.Name}'"
            });
        }

        // Find action
        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
        {
            errors.Add(new ValidationError
            {
                Field = "actionId",
                Message = $"Action '{actionId}' not found in workflow definition"
            });
            return (false, errors);
        }

        // Check if action is enabled
        if (!action.Enabled)
        {
            errors.Add(new ValidationError
            {
                Field = "actionId",
                Message = $"Action '{action.Name}' is disabled"
            });
        }

        // Check if current state is in fromStates
        if (!action.FromStates.Contains(instance.CurrentState))
        {
            errors.Add(new ValidationError
            {
                Field = "currentState",
                Message = $"Action '{action.Name}' cannot be executed from state '{currentState.Name}'"
            });
        }

        // Check if target state exists and is enabled
        var targetState = definition.States.FirstOrDefault(s => s.Id == action.ToState);
        if (targetState == null)
        {
            errors.Add(new ValidationError
            {
                Field = "toState",
                Message = $"Target state '{action.ToState}' not found"
            });
        }
        else if (!targetState.Enabled)
        {
            errors.Add(new ValidationError
            {
                Field = "toState",
                Message = $"Target state '{targetState.Name}' is disabled"
            });
        }

        return (errors.Count == 0, errors);
    }
} 