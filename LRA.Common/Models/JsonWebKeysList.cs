using System.Text.Json.Serialization;

namespace LRA.Common.Models;

public class JsonWebKeysList
{
    [JsonPropertyName("keys")]
    public List<JsonWebKey> Keys { get; set; }
}
