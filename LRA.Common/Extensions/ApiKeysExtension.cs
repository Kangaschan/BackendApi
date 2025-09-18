using LRA.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Common.Extensions;

public static class ApiKeysExtension
{
    public static IServiceCollection ConfigureApiKeys(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiKeysList>(configuration.GetSection("ApiKeysList"));

        return services;
    }
}
