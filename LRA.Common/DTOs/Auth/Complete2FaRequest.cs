namespace LRA.Common.DTOs.Auth;

public class Complete2FaRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string OneTimePassword { get; set; }
}
