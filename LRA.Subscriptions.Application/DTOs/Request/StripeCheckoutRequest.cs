namespace LRA.Subscriptions.Application.DTOs.Request;

public class StripeCheckoutRequest
{
    public required string PriceId { get; set; }
    public required string CustomerId { get; set; }
}
