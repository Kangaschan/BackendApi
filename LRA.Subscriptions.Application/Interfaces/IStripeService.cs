using LRA.Common.Models;
using LRA.Subscriptions.Application.DTOs;
using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.DTOs.Response;
using LRA.Subscriptions.Application.Enums;

namespace LRA.Subscriptions.Application.Interfaces;

public interface IStripeService
{
    Task<string> CreateCheckoutSessionAsync(StripeCheckoutRequest checkoutRequest, CancellationToken cancellationToken);
    IEnumerable<SubscriptionListDto> GetPricesInfo();
    Task<string> CreateStripeCustomerAsync(string mail, CancellationToken cancellationToken);
    Task<string> CreateCustomerPortalSessionAsync(string customerId, CancellationToken cancellationToken);
    public SubscriptionAction GetEventType(StrpeWebHookMessage message);
    Task<SubscriptionInfo> GetSubscriptionInfo(string json);
}
