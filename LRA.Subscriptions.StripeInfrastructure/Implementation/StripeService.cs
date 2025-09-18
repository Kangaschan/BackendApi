using LRA.Common.Models;
using LRA.Subscriptions.Application.DTOs;
using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.DTOs.Response;
using LRA.Subscriptions.Application.Enums;
using LRA.Subscriptions.Application.Interfaces;
using LRA.Subscriptions.StripeInfrastructure.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace LRA.Subscriptions.StripeInfrastructure.Implementation;

public class StripeService : IStripeService 
{
    private readonly StripeOptions _stripeOptions;
    private readonly StripeClient _stripeClient;
    private readonly StripeSubscriptionList _stripeSubscriptionList;
    
    public StripeService(
        IOptions<StripeOptions> stripeOptions,
        IOptions<StripeSubscriptionList> stripeSubscriptionsOptions)
    {
        _stripeOptions = stripeOptions.Value;
        _stripeClient = new StripeClient(_stripeOptions.ApiKey);
        _stripeSubscriptionList = stripeSubscriptionsOptions.Value;
    }
    
    public async Task<string> CreateCheckoutSessionAsync(StripeCheckoutRequest checkoutRequest, CancellationToken cancellationToken)
    {
        var options = new SessionCreateOptions
        {
            Customer = checkoutRequest.CustomerId,
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = checkoutRequest.PriceId,
                    Quantity = 1,
                },
            },
            Mode = "subscription",
            SuccessUrl = _stripeOptions.SuccessUrl,
            CancelUrl = _stripeOptions.CancelUrl,
        };

        var service = new SessionService(_stripeClient);
        Session session = await service.CreateAsync(options);
        return session.Url;
    }
    
    public IEnumerable<SubscriptionListDto> GetPricesInfo()
    {  
        return new List<SubscriptionListDto>
        {
            new SubscriptionListDto
            {
                Subscriptions = _stripeSubscriptionList.Subscriptions
                    .Select(p => new SubscriptionInfoDto
                    {
                        Name = p.Name,
                        Prices = p.Prices
                            .Select(price => new PriceDto
                            {
                                Id = price.Id,
                                Name = price.Name
                            })
                    })
            }
        };
    }

    public async Task<string> CreateStripeCustomerAsync(string mail, CancellationToken cancellationToken)
    {
        var options = new CustomerCreateOptions
        {
            Email = mail
        };
         var service = new CustomerService(_stripeClient);
         var customer = await service.CreateAsync(options);
         return customer.Id;
    }

    public async Task<string> CreateCustomerPortalSessionAsync(string customerId, CancellationToken cancellationToken)
    {
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = customerId,
            ReturnUrl = _stripeOptions.SuccessUrl,
        };

        var service = new Stripe.BillingPortal.SessionService(_stripeClient);
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    public SubscriptionAction GetEventType(StrpeWebHookMessage message)
    {
        var stripeEvent = EventUtility.ConstructEvent(
            message.Json,
            message.Headers,
            _stripeOptions.WebhookSecret,
            throwOnApiVersionMismatch: false
        );
        switch (stripeEvent.Type)
        {
            case "customer.subscription.created":
                return SubscriptionAction.Created;
            case "invoice.paid":
                var invoice = stripeEvent.Data.Object as Invoice;
                if (invoice.BillingReason == "subscription_cycle")
                {
                    return SubscriptionAction.Cycled;
                }
                break;
            case "customer.subscription.updated":
                return SubscriptionAction.Updated;

            case "customer.subscription.deleted":
                return SubscriptionAction.Deleted;
        }
        return SubscriptionAction.Undefined;
    }

    public async Task<SubscriptionInfo> GetSubscriptionInfo(string json)
    {
        var stripeEvent = EventUtility.ParseEvent(json);
        var stripeSubscription = stripeEvent.Data.Object as Subscription;
        return new SubscriptionInfo
        {
            CustomerId = stripeSubscription.CustomerId,
            StripeSubscriptionId = stripeSubscription.Id,
            Status = stripeSubscription.Status,
            PriceId = stripeSubscription.Items.Data[0].Price.Id,
            CurrentPeriodStart = stripeSubscription.Created,
            CurrentPeriodEnd = stripeSubscription.Items.Data[0].CurrentPeriodEnd,
            CancelAtPeriodEnd = stripeSubscription.CancelAtPeriodEnd,
            CreatedAt = stripeSubscription.Created
        };
    }
}
