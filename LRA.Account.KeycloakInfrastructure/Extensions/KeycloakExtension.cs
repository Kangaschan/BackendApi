using LRA.Account.Application.Interfaces;
using LRA.Account.KeycloakInfrastructure.Configuration;
using LRA.Account.KeycloakInfrastructure.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Account.KeycloakInfrastructure.Extensions;

public static class KeycloakExtension
{
    public static IServiceCollection AddKeycloakClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakHttpClientConfig>(configuration.GetSection("KeycloakHttpConfiguration"));
        services.AddHttpClient<IKeycloakServiceClient, KeycloakServiceClient>();
        
        return services;   
    }
}
