using System.Text.Json.Serialization;

namespace LRA.Common.Models;

public class JsonWebKey
{
    [JsonPropertyName("kid")]
    public string KeyId { get; set; }
    [JsonPropertyName("kty")]
    public string KeyType { get; set; }
    [JsonPropertyName("alg")]
    public string Algorithm { get; set; }
    [JsonPropertyName("use")]
    public string PublicKeyUse { get; set; }
    [JsonPropertyName("n")]
    public string Modulus { get; set; }
    [JsonPropertyName("e")]
    public string Exponent { get; set; }
}
