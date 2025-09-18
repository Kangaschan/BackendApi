namespace LRA.Subscriptions.StripeInfrastructure.Configuration;

public class StripeSubscription
{
    public required string Name { get; set; }
    public required List<StripePriceInfo> Prices { get; set; } 
}
