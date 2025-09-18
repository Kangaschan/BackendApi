namespace LRA.Account.Application.DTOs;

public class OneTimePasswordDto
{
    public string Password { get; set; }
    public required string UserEmail { get; set; }
    public DateTime ExpiresAt { get; set; }
}
