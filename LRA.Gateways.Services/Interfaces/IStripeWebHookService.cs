namespace LRA.Gateways.Services.Interfaces;

public interface IStripeWebHookService
{
    Task HandleWebHook(string json, string headers, CancellationToken cancellationToken);
}
