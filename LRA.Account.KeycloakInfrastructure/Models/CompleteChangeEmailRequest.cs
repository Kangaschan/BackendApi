namespace LRA.Account.KeycloakInfrastructure.Models;

public class CompleteChangeEmailRequest
{
    public required string Email { get; set; }
    public required bool EmailVerified { get; set; }
}
