namespace LRA.Subscriptions.Application.DTOs.Request;

public class UserCreateDto
{
    public required string StripeCustomerId { get; set; }
    public required string UserEmail { get; set; }
}
