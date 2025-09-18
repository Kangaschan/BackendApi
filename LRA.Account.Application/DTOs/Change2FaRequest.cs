namespace LRA.Account.Application.DTOs;

public class Change2FaRequest
{
    public required bool IsTwoFactorEnabled { get; set; }
}
