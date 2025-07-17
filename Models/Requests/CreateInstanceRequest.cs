using System.Text.Json.Serialization;

namespace ConfigurableWorkflowEngine.Models.Requests;

public class CreateInstanceRequest
{
    [JsonPropertyName("definitionId")]
    public required string DefinitionId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
} 