using ConfigurableWorkflowEngine.Models.Entities;
using ConfigurableWorkflowEngine.Models.Responses;

namespace ConfigurableWorkflowEngine.Services;

public interface IWorkflowValidationService
{
    Task<(bool IsValid, List<ValidationError> Errors)> ValidateDefinitionAsync(WorkflowDefinition definition);
    Task<(bool IsValid, List<ValidationError> Errors)> ValidateInstanceCreationAsync(string definitionId);
    Task<(bool IsValid, List<ValidationError> Errors)> ValidateActionExecutionAsync(string instanceId, string actionId);
} 