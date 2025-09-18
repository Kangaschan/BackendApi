using LRA.Common.Models;
using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.Interfaces;
using LRA.Subscriptions.Application.Enums;

namespace LRA.Subscriptions.Application.Implementations;

public class StripeWebHookService : IStripeWebHookService
{
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    private readonly IStripeService _stripeService;

    public StripeWebHookService(IUserRepository userRepository, ISubscriptionsRepository subscriptionsRepository,
        IStripeService stripeService)
    {
        _userRepository = userRepository;
        _subscriptionsRepository = subscriptionsRepository;
        _stripeService = stripeService;
    }
    
    public async Task HandleWebHookAsync(StrpeWebHookMessage message, CancellationToken cancellationToken)
    {
        var subscriptionEvent = _stripeService.GetEventType(message);
        switch (subscriptionEvent)
        {
            case SubscriptionAction.Created:
                var createdSubscriptionInfo = await _stripeService.GetSubscriptionInfo(message.Json);
                var userForCreatedSubscription =await _userRepository.GetByCustomerIdAsync(createdSubscriptionInfo.CustomerId,cancellationToken);
                var subscriptionCreateDto = new SubscriptionCreateDto
                {
                    StripeSubscriptionId = createdSubscriptionInfo.StripeSubscriptionId,
                    CancelAtPeriodEnd = createdSubscriptionInfo.CancelAtPeriodEnd,
                    PriceId = createdSubscriptionInfo.PriceId,
                    Status = createdSubscriptionInfo.Status,
                    CurrentPeriodStart = createdSubscriptionInfo.CurrentPeriodStart,
                    CurrentPeriodEnd = createdSubscriptionInfo.CurrentPeriodEnd,
                    UserId = userForCreatedSubscription.Id,
                };
                await _subscriptionsRepository.CreateAsync(subscriptionCreateDto, cancellationToken);
                break;
            case SubscriptionAction.Cycled:
            case SubscriptionAction.Updated:
                var updatedSubscriptionInfo = await _stripeService.GetSubscriptionInfo(message.Json);
                var userForUpdatedSubscription =await _userRepository.GetByCustomerIdAsync(updatedSubscriptionInfo.CustomerId,cancellationToken);
                var subscriptionUpdateDto = new SubscriptionUpdateDto
                {
                    CancelAtPeriodEnd = updatedSubscriptionInfo.CancelAtPeriodEnd,
                    PriceId = updatedSubscriptionInfo.PriceId,
                    Status = updatedSubscriptionInfo.Status,
                    CurrentPeriodStart = updatedSubscriptionInfo.CurrentPeriodStart,
                    CurrentPeriodEnd = updatedSubscriptionInfo.CurrentPeriodEnd,
                };
                _subscriptionsRepository.UpdateAsync(userForUpdatedSubscription.CurrentSubscription, subscriptionUpdateDto, cancellationToken);
                break;
            case SubscriptionAction.Deleted:
                var deletedSubscription = await _stripeService.GetSubscriptionInfo(message.Json);
                var userForDeletedSubscription =await _userRepository.GetByCustomerIdAsync(deletedSubscription.CustomerId,cancellationToken);
                var deletedSubscriptionUpdateDto = new SubscriptionUpdateDto
                {
                    CancelAtPeriodEnd = deletedSubscription.CancelAtPeriodEnd,
                    PriceId = deletedSubscription.PriceId,
                    Status = deletedSubscription.Status,
                    CurrentPeriodStart = deletedSubscription.CurrentPeriodStart,
                    CurrentPeriodEnd = deletedSubscription.CurrentPeriodEnd,
                };
                await _subscriptionsRepository.UpdateAsync(userForDeletedSubscription.CurrentSubscription, deletedSubscriptionUpdateDto, cancellationToken);
                break;
        }
    }
}
