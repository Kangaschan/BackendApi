using LRA.Subscriptions.Application.Implementations;
using LRA.Subscriptions.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Subscriptions.Application.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ISubscriptionsService, SubscriptionsService>();
        return services;
    }
}
