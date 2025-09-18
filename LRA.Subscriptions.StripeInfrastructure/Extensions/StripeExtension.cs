using LRA.Subscriptions.Application.Interfaces;
using LRA.Subscriptions.StripeInfrastructure.Configuration;
using LRA.Subscriptions.StripeInfrastructure.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Subscriptions.StripeInfrastructure.Extensions;

public static class StripeExtension
{
    public static IServiceCollection ConfigureStripeSetting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"))
            .Configure<StripeSubscriptionList>(configuration.GetSection("StripeSubscriptions"));
        return services;
    }

    public static IServiceCollection ConfigureStripeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IStripeService, StripeService>();
        return services;
    }
}
