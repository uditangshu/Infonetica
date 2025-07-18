using System.Text.Json.Serialization;

namespace ConfigurableWorkflowEngine.Models.Requests;

public class ExecuteActionRequest
{
    [JsonPropertyName("actionId")]
    public required string ActionId { get; set; }

    [JsonPropertyName("executedBy")]
    public string? ExecutedBy { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}