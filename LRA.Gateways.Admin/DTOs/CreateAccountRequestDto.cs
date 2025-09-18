namespace LRA.Gateways.Admin.DTOs;

public class CreateAccountRequestDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? Phone { get; set; }
    public ICollection<string>? Roles { get; set; } 
    public required bool IsTemporaryPassword { get; set; }
}
