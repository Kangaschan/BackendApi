namespace LRA.Account.Application.DTOs;

public class CompleteResetPasswordRequest
{
    public required string Email { get; set; }
    public required string OneTimePassword { get; set; }
    public required string NewPassword { get; set; }
}
