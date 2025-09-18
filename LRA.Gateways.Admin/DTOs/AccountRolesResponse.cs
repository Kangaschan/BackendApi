namespace LRA.Gateways.Admin.DTOs;

public class AccountRolesResponse
{
    public required bool IsAdmin { get; set; }
    public required bool IsSuperAdmin { get; set; }
    public string? Email { get; set; }
}
