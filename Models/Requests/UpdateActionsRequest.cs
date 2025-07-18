using System.Text.Json.Serialization;
using ConfigurableWorkflowEngine.Models.Entities;

namespace ConfigurableWorkflowEngine.Models.Requests;

public class UpdateActionsRequest
{
    [JsonPropertyName("add")]
    public List<WorkflowAction>? Add { get; set; } = new();

    [JsonPropertyName("update")]
    public List<WorkflowAction>? Update { get; set; } = new();

    [JsonPropertyName("remove")]
    public List<string>? Remove { get; set; } = new();
} 