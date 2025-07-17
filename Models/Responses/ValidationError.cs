using System.Text.Json.Serialization;

namespace ConfigurableWorkflowEngine.Models.Responses;

public class ValidationError
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
} 