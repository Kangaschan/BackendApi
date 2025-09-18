using LRA.Common.Exceptions;
using LRA.Subscriptions.Application.DTOs;
using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.DTOs.Response;
using LRA.Subscriptions.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LRA.Subscriptions.Application.Implementations;

public class SubscriptionsService : ISubscriptionsService
{
    private readonly IStripeService _stripeService;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionsRepository _subscriptionsRepository;
    
    public SubscriptionsService(IStripeService stripeService, IUserRepository userRepository, ISubscriptionsRepository subscriptionsRepository)
    {
        _stripeService = stripeService;
        _userRepository = userRepository;
        _subscriptionsRepository = subscriptionsRepository;
    }

    private async Task<string> CreateStripeCustomer(string email, CancellationToken cancellationToken)
    {
        var customerId = await _stripeService.CreateStripeCustomerAsync(email, cancellationToken);
        return customerId;
    }
    
    public IEnumerable<SubscriptionListDto> GetPricesInfo()
    {
        var subscriptionsInfo = _stripeService.GetPricesInfo();
        return subscriptionsInfo;
    }

    public async Task<string> ManageSubscriptionsAsync(PortalRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByMailAsync(request.Email, cancellationToken);
        if (user.CurrentSubscription == null)
        {
            throw new SubscriptionException("user have no active subscription.");
        }
        var subscription = await _subscriptionsRepository.GetByIdAsync(user.CurrentSubscription, cancellationToken);
        if (subscription.CurrentPeriodEnd < DateTime.UtcNow && subscription.Status == "active")
        {
            throw new SubscriptionException("user have no active subscription.");
        }
        var redirect = await _stripeService.CreateCustomerPortalSessionAsync(user.StripeCustomerId, cancellationToken);
        return redirect;
    }

    public async Task<string> BuySubscriptionsAsync(CheckoutRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByMailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            var customerId = await CreateStripeCustomer(request.Email, cancellationToken);
            var userDto = new UserCreateDto
            {
                UserEmail = request.Email,
                StripeCustomerId = customerId
            };
            
            await _userRepository.CreateUserAsync(userDto, cancellationToken);
            user = new UserResopnseDto
            {
                StripeCustomerId = customerId
            };
        }
        if(user.CurrentSubscription != null)
        {
            throw new SubscriptionException("user have active subscription.");
        }

        var stripeCheckoutRequest = new StripeCheckoutRequest
        {
            CustomerId = user.StripeCustomerId,
            PriceId = request.PriceId,
        };
        var redirectUrl = await _stripeService.CreateCheckoutSessionAsync(stripeCheckoutRequest, cancellationToken);
        return redirectUrl;
    }

    public async Task<CheckActiveSubscriptionResponse> CheckActiveSubscriptionsAsync(
        CheckActiveSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByMailAsync(request.Email, cancellationToken);
        var subscription = await _subscriptionsRepository.GetByIdAsync(user.CurrentSubscription, cancellationToken);
        if (subscription.CurrentPeriodEnd > DateTime.UtcNow && subscription.Status == "active")
        {
            var checkActiveSubscriptionResponse =  new CheckActiveSubscriptionResponse
            {
                IsActive = true
            };
            return checkActiveSubscriptionResponse;
        }
        else
        {
            var res =  new CheckActiveSubscriptionResponse
            {
                IsActive = false
            };
            return res;
        }
    }
}
