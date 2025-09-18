namespace LRA.Subscriptions.Application.DTOs.Response;

public class SubscriptionInfo
{
    public Guid Id { get; set; }
    public required string StripeSubscriptionId { get; set; }
    public required string Status { get; set; }
    public required string PriceId { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public required string CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
}
