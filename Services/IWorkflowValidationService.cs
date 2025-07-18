using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;

namespace ConfigurableWorkflowEngine.Services;

public interface IWorkflowValidationService
{
    List<ValidationError> ValidateDefinition(WorkflowDefinition definition);
    List<ValidationError> ValidateAction(WorkflowDefinition definition, WorkflowInstance instance, string actionId);
} 