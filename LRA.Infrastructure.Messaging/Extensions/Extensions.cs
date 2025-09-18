using LRA.Infrastructure.Messaging.Configuration;
using LRA.Infrastructure.Messaging.Implementations;
using LRA.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Infrastructure.Messaging.Extensions;

public static class Extensions
{
    public static IServiceCollection AddProducer(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.Configure<KafkaProducerConfiguration>(configurationSection);
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        
        return services;
    }

    public static IServiceCollection AddConsumer(this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.Configure<KafkaConsumerConfiguration>(configurationSection);
        services.AddSingleton<IKafkaConsumer, KafkaConsumerBase>();
        
        return services;
    }
}
