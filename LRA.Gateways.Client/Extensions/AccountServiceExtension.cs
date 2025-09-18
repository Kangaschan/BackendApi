using LRA.Gateways.Client.Configuration;
using LRA.Gateways.Client.Implementations;
using LRA.Gateways.Client.Interfaces;
using Microsoft.Extensions.Options;

namespace LRA.Gateways.Client.Extensions;

public static class AccountServiceExtension
{
    public static IServiceCollection ConfigureAccountService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AccountHttpClientConfig>(configuration.GetSection("AccountHttpClientConfig"));
        services.AddHttpClient<IAccountServiceClient, AccountServiceClient>();
      
        return services;
    }    
}
