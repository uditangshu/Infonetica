using System.Text.Json.Serialization;
using ConfigurableWorkflowEngine.Models.Entities;

namespace ConfigurableWorkflowEngine.Models.Requests;

public class UpdateStatesRequest
{
    [JsonPropertyName("add")]
    public List<WorkflowState>? Add { get; set; } = new();

    [JsonPropertyName("update")]
    public List<WorkflowState>? Update { get; set; } = new();

    [JsonPropertyName("remove")]
    public List<string>? Remove { get; set; } = new();
} 