using LRA.Common.Models;

namespace LRA.Subscriptions.Domain.Entities;

public class UserEntity : IBaseEntity
{
    public Guid Id { get; set; }
    
    public required string StripeCustomerId { get; set; }
    
    public required string UserEmail { get; set; }
 
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }

    public SubscriptionEntity? CurrentSubscription { get; set; }
}
