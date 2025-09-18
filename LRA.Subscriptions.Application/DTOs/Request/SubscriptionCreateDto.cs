namespace LRA.Subscriptions.Application.DTOs.Request;

public class SubscriptionCreateDto
{
    public Guid UserId { get; set; } 
    public required string StripeSubscriptionId { get; set; } 
    public required string PriceId { get; set; }  
    public required string Status { get; set; }
    public required DateTime CurrentPeriodStart { get; set; } 
    public required DateTime CurrentPeriodEnd { get; set; }
    public required bool CancelAtPeriodEnd { get; set; } 
}
