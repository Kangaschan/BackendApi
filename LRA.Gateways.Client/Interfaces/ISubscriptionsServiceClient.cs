using LRA.Gateways.Client.DTOs;

namespace LRA.Gateways.Client.Interfaces;

public interface ISubscriptionsServiceClient
{
    Task<IEnumerable<SubscriptionListDto>> GetPricesAsync(CancellationToken cancellationToken);
    Task<string> BuySubscriptionsAsync(ChoosedSubscriptionDto request, string mail, CancellationToken cancellationToken);
    Task<string> ManageSubscriptionsAsync(string mail, CancellationToken cancellationToken);
    Task<bool> CheckSubscriptionsExists(string mail, CancellationToken cancellationToken);
}
