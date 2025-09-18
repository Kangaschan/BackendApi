namespace LRA.Account.Application.DTOs;

public class UpdateAccountCredentialsRequest
{
    public required bool IsTemporaryPassword { get; set; } 
    public required bool IsTemporaryPasswordUsed { get; set; } 
    public DateTime? ExpiresAt { get; set; }
}
