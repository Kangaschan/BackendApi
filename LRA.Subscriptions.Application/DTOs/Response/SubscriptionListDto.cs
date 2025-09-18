namespace LRA.Subscriptions.Application.DTOs.Response;

public class SubscriptionListDto
{
    public required IEnumerable<SubscriptionInfoDto> Subscriptions { get; set; } 
}
