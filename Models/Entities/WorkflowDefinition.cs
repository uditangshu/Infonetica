using System.Text.Json.Serialization;

namespace ConfigurableWorkflowEngine.Models.Entities;

public class WorkflowDefinition
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("states")]
    public required List<WorkflowState> States { get; set; }

    [JsonPropertyName("actions")]
    public required List<WorkflowAction> Actions { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;
} 