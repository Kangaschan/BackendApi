using System.Text.Json;
using LRA.Common.Models;
using LRA.Infrastructure.Messaging.Configuration;
using LRA.Infrastructure.Messaging.Interfaces;
using LRA.Subscriptions.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LRA.Subscriptions.Application.Implementations;

public class WebHooksKafkaConsumer : BackgroundService
{
    private readonly ILogger<WebHooksKafkaConsumer> _logger;
    private readonly IKafkaConsumer _kafkaConsumer;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public WebHooksKafkaConsumer(ILogger<WebHooksKafkaConsumer> logger, IKafkaConsumer kafkaConsumer, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        {
            _kafkaConsumer.Subscribe();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _kafkaConsumer.Consume<StrpeWebHookMessage>(cancellationToken);
                    _logger.LogInformation("Received message: {Message}", result.ToString());
                    await ProcessMessageAsync(result, cancellationToken);
                    await Task.Delay(200, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in Kafka consumer. Retrying in 500ms");
                    await Task.Delay(500, cancellationToken);
                }
            }
        }

    }

    private async Task ProcessMessageAsync(StrpeWebHookMessage message, CancellationToken cancellationToken)
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var stripeWebHookService = scope.ServiceProvider.GetRequiredService<IStripeWebHookService>();
                await stripeWebHookService.HandleWebHookAsync(message, cancellationToken);
            }
            _logger.LogInformation("Successfully processed webhook event {EventId}", message.ToString());
        }
        catch(InvalidCastException  ex)
        {
            _logger.LogWarning("Received unexpected message format. Type: {MessageType}, Content: {MessageContent}",
                message?.GetType().Name,
                message);   
        }
    }
    
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping WebHooks Kafka consumer");
        _kafkaConsumer.Close();
        return base.StopAsync(cancellationToken);
    }
}
