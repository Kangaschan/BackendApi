using System.Text.Json.Serialization;
using LRA.Common.Models;

namespace LRA.Account.Application.DTOs;

public class KeycloakAccountCreateRequest
{
    [JsonPropertyName ("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName ("enabled")]
    public bool Enabled { get; set; }
    
    [JsonPropertyName ("credentials")]
    public required List<KeycloakCredential> Credentials { get; set; }
}
