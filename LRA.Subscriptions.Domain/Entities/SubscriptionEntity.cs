using LRA.Common.Models;

namespace LRA.Subscriptions.Domain.Entities;

public class SubscriptionEntity : IBaseEntity
{
    public Guid Id { get; set; }

    public required string StripeSubscriptionId { get; set; }

    public string? Status { get; set; }

    public required string PriceId { get; set; }

    public DateTime CurrentPeriodStart { get; set; }

    public DateTime? CurrentPeriodEnd { get; set; }

    public bool? CancelAtPeriodEnd { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }

    public Guid UserId { get; set; }
    
    public UserEntity? User { get; set; }
}
