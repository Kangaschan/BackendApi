namespace LRA.Common.DTOs;

public class RegisterAccountRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PasswordConfirmation { get; set; }
}
