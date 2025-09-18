namespace LRA.Gateways.Admin.Configurations;

public class AccountHttpClientConfig
{
    public required string Route { get; set; }
    public required string BaseUrl { get; set; }
    public required string ApiKey { get; set; }
}
