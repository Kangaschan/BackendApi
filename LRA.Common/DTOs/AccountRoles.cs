namespace LRA.Common.DTOs;

public class AccountRoles
{
    public required ICollection<string> Roles { get; set; }
    public string? Email { get; set; } 
}
