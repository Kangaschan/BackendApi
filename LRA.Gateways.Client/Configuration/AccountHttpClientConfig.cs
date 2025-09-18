namespace LRA.Gateways.Client.Configuration;

public class AccountHttpClientConfig
{
    public required string Route { get; set; }
    public required string BaseUrl { get; set; }
    public required string ApiKey { get; set; }
}
