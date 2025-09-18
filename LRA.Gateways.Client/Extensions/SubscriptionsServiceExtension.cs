using LRA.Gateways.Client.Configuration;
using LRA.Gateways.Client.Implementations;
using LRA.Gateways.Client.Interfaces;

namespace LRA.Gateways.Client.Extensions;

public static class SubscriptionsServiceExtension
{
    public static IServiceCollection ConfigureSubscriptionsService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SubscriptionsHttpClientConfig>(configuration.GetSection("SubscriptionsHttpClientConfig"));
        services.AddHttpClient<ISubscriptionsServiceClient, SubscriptionsServiceClient>();
      
        return services;
    }    
}
