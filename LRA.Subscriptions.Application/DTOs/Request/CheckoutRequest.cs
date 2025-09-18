namespace LRA.Subscriptions.Application.DTOs.Request;

public class CheckoutRequest
{
    public required string PriceId { get; set; }
    public required string Email { get; set; }
}
