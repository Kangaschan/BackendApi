namespace LRA.Account.Application.DTOs;

public class ChangeFullNameRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
