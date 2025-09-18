namespace LRA.Gateways.Client.DTOs;

public class SubscriptionDto
{
    public required string Name { get; set; }
    public required IEnumerable<PriceDto> Prices { get; set; } 
}
