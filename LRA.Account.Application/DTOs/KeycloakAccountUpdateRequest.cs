namespace LRA.Account.Application.DTOs;

public class KeycloakAccountUpdateRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public bool Enabled { get; set; }
}
