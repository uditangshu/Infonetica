using System.Text.Json.Serialization;

namespace ConfigurableWorkflowEngine.Models.Entities;

public class WorkflowState
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("isInitial")]
    public bool IsInitial { get; set; }

    [JsonPropertyName("isFinal")]
    public bool IsFinal { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("description")]
    public string? Description { get; set; }
} 