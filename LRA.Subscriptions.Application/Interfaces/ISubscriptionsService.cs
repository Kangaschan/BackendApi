using LRA.Subscriptions.Application.DTOs;
using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.DTOs.Response;

namespace LRA.Subscriptions.Application.Interfaces;

public interface ISubscriptionsService
{
    Task<string> BuySubscriptionsAsync(CheckoutRequest request, CancellationToken cancellationToken);
    IEnumerable<SubscriptionListDto> GetPricesInfo();
    Task<String> ManageSubscriptionsAsync(PortalRequest request, CancellationToken cancellationToken);
    Task<CheckActiveSubscriptionResponse> CheckActiveSubscriptionsAsync(CheckActiveSubscriptionRequest request, CancellationToken cancellationToken);
}
