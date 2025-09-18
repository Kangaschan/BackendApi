namespace LRA.Subscriptions.Application.DTOs.Response;

public class UserResopnseDto
{
    public Guid Id { get; set; }
    public string StripeCustomerId { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public Guid? CurrentSubscription { get; set; }
}
