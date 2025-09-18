namespace LRA.Subscriptions.Application.DTOs.Response;

public class SubscriptionInfoDto
{
    public required string Name { get; set; }
    public required IEnumerable<PriceDto> Prices { get; set; } 
}
