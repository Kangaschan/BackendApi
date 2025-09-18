namespace LRA.Account.Application.DTOs;

public class EmailConfirmationTokenDto
{
    public string? Token { get; set; }
    public required string UserEmail { get; set; }
    public DateTime ExpiresAt { get; set; }
}
