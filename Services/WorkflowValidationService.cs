using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;
using ConfigurableWorkflowEngine.Core.Interfaces;

namespace ConfigurableWorkflowEngine.Services;

public class WorkflowValidationService : IWorkflowValidationService
{
    public List<ValidationError> ValidateDefinition(WorkflowDefinition definition)
    {
        var errors = new List<ValidationError>();

        // Check basic requirements
        if (string.IsNullOrEmpty(definition.Id))
            errors.Add(new ValidationError { Message = "Definition ID is required" });
        if (string.IsNullOrEmpty(definition.Name))
            errors.Add(new ValidationError { Message = "Definition name is required" });

        // Check states
        if (!definition.States.Any())
            errors.Add(new ValidationError { Message = "At least one state is required" });

        var initialStates = definition.States.Where(s => s.IsInitial).ToList();
        if (initialStates.Count != 1)
            errors.Add(new ValidationError { Message = "Exactly one initial state is required" });

        // Check state IDs are unique
        var duplicateStates = definition.States.GroupBy(s => s.Id).Where(g => g.Count() > 1);
        foreach (var group in duplicateStates)
            errors.Add(new ValidationError { Message = $"Duplicate state ID: {group.Key}" });

        // Check action IDs are unique
        var duplicateActions = definition.Actions.GroupBy(a => a.Id).Where(g => g.Count() > 1);
        foreach (var group in duplicateActions)
            errors.Add(new ValidationError { Message = $"Duplicate action ID: {group.Key}" });

        // Check action references valid states
        var stateIds = definition.States.Select(s => s.Id).ToHashSet();
        foreach (var action in definition.Actions)
        {
            if (!stateIds.Contains(action.ToState))
                errors.Add(new ValidationError { Message = $"Action '{action.Id}' references invalid toState: {action.ToState}" });
            
            foreach (var fromState in action.FromStates)
            {
                if (!stateIds.Contains(fromState))
                    errors.Add(new ValidationError { Message = $"Action '{action.Id}' references invalid fromState: {fromState}" });
            }
        }

        return errors;
    }

    public List<ValidationError> ValidateAction(WorkflowDefinition definition, WorkflowInstance instance, string actionId)
    {
        var errors = new List<ValidationError>();

        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
        {
            errors.Add(new ValidationError { Message = $"Action '{actionId}' not found" });
            return errors;
        }

        if (!action.Enabled)
            errors.Add(new ValidationError { Message = $"Action '{actionId}' is disabled" });

        if (!action.FromStates.Contains(instance.CurrentState))
            errors.Add(new ValidationError { Message = $"Action '{actionId}' cannot be executed from state '{instance.CurrentState}'" });

        return errors;
    }
} 