using System.Text.Json.Serialization;

namespace LRA.Account.Application.DTOs;

public class KeycloakIntrospectionResponse
{
    [JsonPropertyName("active")]
    public bool Active { get; set; }
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [JsonPropertyName("preferred_username")]
    public string? PreferredUsername { get; set; }
    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }
    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }
    [JsonPropertyName("email_verified")]
    public bool? EmailVerified { get; set; }
}
