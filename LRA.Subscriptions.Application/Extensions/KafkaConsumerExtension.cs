using LRA.Infrastructure.Messaging.Configuration;
using LRA.Infrastructure.Messaging.Implementations;
using LRA.Infrastructure.Messaging.Interfaces;
using LRA.Subscriptions.Application.Implementations;
using LRA.Subscriptions.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Subscriptions.Application.Extensions;

public static class KafkaConsumerExtension
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    { 
        services.AddScoped<IStripeWebHookService, StripeWebHookService>();
        services.Configure<KafkaConsumerConfiguration>(configuration.GetSection("Kafka:SubscriptionsConsume"));
        services.AddSingleton<IKafkaConsumer, KafkaConsumerBase>();
        services.AddHostedService<WebHooksKafkaConsumer>();
        
        return services;   
    }
}
