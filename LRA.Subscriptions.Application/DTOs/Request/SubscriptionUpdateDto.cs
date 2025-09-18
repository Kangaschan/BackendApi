namespace LRA.Subscriptions.Application.DTOs.Request;

public class SubscriptionUpdateDto
{
    public string? Status { get; set; } 
    public DateTime? CurrentPeriodStart { get; set; }  
    public DateTime? CurrentPeriodEnd { get; set; }  
    public bool? CancelAtPeriodEnd { get; set; }  
    public string? PriceId { get; set; }  
}
