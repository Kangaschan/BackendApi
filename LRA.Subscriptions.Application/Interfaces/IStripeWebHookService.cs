using LRA.Common.Models;

namespace LRA.Subscriptions.Application.Interfaces;

public interface IStripeWebHookService
{
    Task HandleWebHookAsync(StrpeWebHookMessage message, CancellationToken cancellationToken);
}
