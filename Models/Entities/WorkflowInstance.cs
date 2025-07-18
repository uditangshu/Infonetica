using System.Text.Json.Serialization;

namespace ConfigurableWorkflowEngine.Models.Entities;

public class WorkflowInstance
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("definitionId")]
    public required string DefinitionId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("currentState")]
    public required string CurrentState { get; set; }

    [JsonPropertyName("history")]
    public List<HistoryEntry> History { get; set; } = new();

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("status")]
    public WorkflowStatus Status { get; set; } = WorkflowStatus.Active;
}

public class HistoryEntry
{
    [JsonPropertyName("stateId")]
    public required string StateId { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("actionId")]
    public string? ActionId { get; set; }

    [JsonPropertyName("executedBy")]
    public string? ExecutedBy { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

public enum WorkflowStatus
{
    Active,
    Completed,
    Cancelled
} 