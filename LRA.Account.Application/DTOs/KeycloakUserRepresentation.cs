using System.Text.Json.Serialization;

namespace LRA.Account.Application.DTOs;

public class KeycloakUserRepresentation
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("createdTimestamp")]
    public long CreatedTimestamp { get; set; }
}
