namespace LRA.Account.KeycloakInfrastructure.Configuration;

public class KeycloakHttpClientConfig
{
    public required string BaseUrl { get; set; }
    public required string AdminClientSecret { get; set; }
    public required string LoginClientSecret { get; set; }
    public required string AdminRoute { get; set; }
    public required string AdminTokenRoute { get; set; }
    public required string LoginRoute { get; set; }
    public required string CertsRoute { get; set; }
    public required string CallbackRoute { get; set; }
    public required string GoogleLoginRoute { get; set; }
}
