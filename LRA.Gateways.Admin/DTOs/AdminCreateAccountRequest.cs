namespace LRA.Gateways.Admin.DTOs;

public class AdminCreateAccountRequest
{
    public required string Email { get; set; }
    public ICollection<string>? Roles { get; set; } 
}
