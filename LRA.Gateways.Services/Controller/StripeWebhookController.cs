using LRA.Gateways.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LRA.Gateways.Services.Controller;

[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebhookController : ControllerBase
{
    private readonly IStripeWebHookService _stripeWebHookService;

    public StripeWebhookController(IStripeWebHookService stripeWebHookService)
    {
        _stripeWebHookService = stripeWebHookService;
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook(CancellationToken cancellationToken)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var header = Request.Headers["Stripe-Signature"];
        await _stripeWebHookService.HandleWebHook(json,header, cancellationToken);
        return Ok();
    }
}
