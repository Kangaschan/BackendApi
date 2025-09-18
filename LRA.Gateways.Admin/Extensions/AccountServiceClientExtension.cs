using LRA.Gateways.Admin.Configurations;
using LRA.Gateways.Admin.Implementations;
using LRA.Gateways.Admin.Interfaces;

namespace LRA.Gateways.Admin.Extensions;

public static class AccountServiceClientExtension
{
    public static IServiceCollection ConfigureAccountService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AccountHttpClientConfig>(configuration.GetSection("AccountHttpClientConfig"));
        services.AddHttpClient<IAccountServiceClient, AccountServiceClient>();
      
        return services;
    }    
}
