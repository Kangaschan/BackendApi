namespace LRA.Gateways.Client.DTOs;

public class CheckoutRequest
{
    public required string PriceId { get; set; }
    public required string Email { get; set; }
}
