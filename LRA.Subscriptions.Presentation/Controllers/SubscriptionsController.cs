using LRA.Common.Exceptions;
using LRA.Subscriptions.Application.DTOs;
using LRA.Subscriptions.Application.DTOs.Request;
using LRA.Subscriptions.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Subscriptions.Presentation.Controllers;

[Route("api/subscriptions")]
[ApiController]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionsService _subscriptionsService;
    
    public SubscriptionsController(ISubscriptionsService subscriptionsService)
    {
        _subscriptionsService = subscriptionsService;
    }
    
    [HttpPost("checkout-url")]
    [Authorize]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var sessionUrl = await _subscriptionsService.BuySubscriptionsAsync(request, cancellationToken);
            return Ok(sessionUrl);
        }
        catch (SubscriptionException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("prices")]
    [Authorize]
    public IActionResult GetPrices()
    {
        var prices = _subscriptionsService.GetPricesInfo();
        return Ok(prices);
    }
    
    [HttpPost("portal-url")]
    [Authorize]
    public async Task<IActionResult> CreatePortalSession([FromBody] PortalRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var portalUrl = await _subscriptionsService.ManageSubscriptionsAsync(request, cancellationToken);
            return Ok(portalUrl); 
        }
        catch (SubscriptionException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("check")]
    [Authorize]
    public async Task<IActionResult> CheckActiveSubscriptionAsync([FromBody] CheckActiveSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var res = await _subscriptionsService.CheckActiveSubscriptionsAsync(request, cancellationToken);
        return Ok(res);
    }
    
}
