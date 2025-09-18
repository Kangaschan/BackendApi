using LRA.Email.Configuration;
using LRA.Email.Implementations;
using LRA.Email.Interfaces;
using LRA.Infrastructure.Messaging.Configuration;
using LRA.Infrastructure.Messaging.Implementations;
using LRA.Infrastructure.Messaging.Interfaces;

namespace LRA.Email.Extensions;

public static class SmtpExtension
{
    public static IServiceCollection ConfigureSmtp(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpSettings>(
            configuration.GetSection("SmtpSettings"));
        
        services.AddTransient<ISmtpService, SmtpService>();

        return services;
    }
    
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaConsumerConfiguration>(configuration.GetSection("Kafka:EmailConsume"));
        services.AddSingleton<IKafkaConsumer, KafkaConsumerBase>();
        services.AddHostedService<EmailKafkaConsumer>();
        
        return services;   
    }
}
