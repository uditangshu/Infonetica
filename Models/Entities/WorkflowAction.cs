using System.Text.Json.Serialization;

namespace ConfigurableWorkflowEngine.Models.Entities;

public class WorkflowAction
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("fromStates")]
    public required List<string> FromStates { get; set; }

    [JsonPropertyName("toState")]
    public required string ToState { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
} 