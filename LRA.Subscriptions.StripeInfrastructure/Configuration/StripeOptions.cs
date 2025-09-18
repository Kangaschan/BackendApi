namespace LRA.Subscriptions.StripeInfrastructure.Configuration;

public class StripeOptions
{
    public required string ApiKey { get; set; }
    public required string WebhookSecret { get; set; }
    public required string SuccessUrl { get; set; }
    public required string CancelUrl { get; set; }
}
