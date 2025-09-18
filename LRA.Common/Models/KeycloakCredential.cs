using System.Text.Json.Serialization;

namespace LRA.Common.Models;

public class KeycloakCredential
{
    [JsonPropertyName ("type")]
    public required string type { get; set; }
    
    [JsonPropertyName ("value")]
    public required string value { get; set; }
    
    [JsonPropertyName ("temporary")]
    public required bool temporary { get; set; }
}
