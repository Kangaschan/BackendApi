namespace LRA.Account.Application.DTOs;

public class ChangeEmailRequest
{
    public required string NewEmail { get; set; }
    public required string Password { get; set; }
}
