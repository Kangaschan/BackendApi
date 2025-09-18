using System.Net;
using LRA.Common.Exceptions;
using LRA.Gateways.Client.Configuration;
using LRA.Gateways.Client.DTOs;
using LRA.Gateways.Client.Interfaces;
using Microsoft.Extensions.Options;

namespace LRA.Gateways.Client.Implementations;

public class SubscriptionsServiceClient : ISubscriptionsServiceClient
{
    private readonly SubscriptionsHttpClientConfig _subscriptionsHttpClientConfig;
    private readonly HttpClient _httpClient;
    
    public SubscriptionsServiceClient(
        IOptions<SubscriptionsHttpClientConfig> subscriptionsRouteConfig,
        HttpClient httpClient)
    {
        _subscriptionsHttpClientConfig = subscriptionsRouteConfig.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_subscriptionsHttpClientConfig.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _subscriptionsHttpClientConfig.ApiKey);
    }
    
    public async Task<IEnumerable<SubscriptionListDto>> GetPricesAsync(CancellationToken cancellationToken)
    {
        var requestPath = $"{_subscriptionsHttpClientConfig.Route}prices";
        using (var response = await _httpClient.GetAsync(requestPath, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error getting Subscription list. Code: {response.StatusCode}. service answer: {errorContent}");
            }
            var subscriptionList =
                await response.Content.ReadFromJsonAsync<IEnumerable<SubscriptionListDto>>(cancellationToken);
            return subscriptionList;
        }
    }
    
    public async Task<string> BuySubscriptionsAsync(ChoosedSubscriptionDto request, string mail, CancellationToken cancellationToken)
    {
        var requestPath = $"{_subscriptionsHttpClientConfig.Route}checkout-url";

        var checkoutRequest = new CheckoutRequest
        {
            Email = mail,
            PriceId = request.PriceId,
        };
        
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, checkoutRequest, cancellationToken))
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new SubscriptionException("user already purchased subscription");
            }
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Subscription purchase error. Code: {response.StatusCode}. service answer: {errorContent}");
            }
            var sessionUrl = await response.Content.ReadAsStringAsync(cancellationToken);
            return sessionUrl;
        }  
    }

    public async Task<string> ManageSubscriptionsAsync(string mail, CancellationToken cancellationToken)
    {
        var requestPath = $"{_subscriptionsHttpClientConfig.Route}portal-url";

        var checkoutRequest = new PortalRequest
        {
            Email = mail
        };
        
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, checkoutRequest, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error creating portal. Code: {response.StatusCode}. service answer: {errorContent}");
            }

            var sessionUrl = await response.Content.ReadAsStringAsync(cancellationToken);
            return sessionUrl;
        }
    }

    public async Task<bool> CheckSubscriptionsExists(string mail, CancellationToken cancellationToken)
    {
        var requestPath = $"{_subscriptionsHttpClientConfig.Route}check";
        var request = new CheckActiveSubscriptionRequest
        {
            Email = mail,
        };
        using (var response = await _httpClient.PostAsJsonAsync(requestPath, request, cancellationToken))
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Subscription check error. Code: {response.StatusCode}. service answer: {errorContent}");
            }

            var checkActiveSubscriptionResponse =
                await response.Content.ReadFromJsonAsync<CheckActiveSubscriptionResponse>(cancellationToken);

            return checkActiveSubscriptionResponse.IsActive;
        }
    }
}
