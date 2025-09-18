using System.Text.Json.Serialization;

namespace LRA.Account.Application.DTOs;

public class KeycloakAccountEnableteDto
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}
