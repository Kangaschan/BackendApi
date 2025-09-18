using LRA.Common.Models;
using LRA.Gateways.Services.Interfaces;
using LRA.Infrastructure.Messaging.Interfaces;

namespace LRA.Gateways.Services.Implementaions;

public class StripeWebHookService : IStripeWebHookService
{
    private readonly IKafkaProducer _producer;

    public StripeWebHookService(IKafkaProducer producer)
    {
        _producer = producer;
    }
    
    public async Task HandleWebHook(string json, string headers, CancellationToken cancellationToken)
    {
        var message = new StrpeWebHookMessage
        {
            Json = json,
            Headers = headers
        };
        await _producer.ProduceAsync(message, cancellationToken);
    }
}
