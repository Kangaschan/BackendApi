namespace LRA.Subscriptions.Application.DTOs.Response;

public class SubscriptionResponseDto
{
    public Guid Id { get; set; }
    public required string StripeSubscriptionId { get; set; }
    public string? Status { get; set; }
    public required string PriceId { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public bool? CancelAtPeriodEnd { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid? UserId { get; set; }
}
