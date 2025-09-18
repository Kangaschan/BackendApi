using LRA.Gateways.Admin.Implementations;
using LRA.Gateways.Admin.Interfaces;

namespace LRA.Gateways.Admin.Extensions;

public static class CurrentUserContextExtension
{
    public static IServiceCollection ConfigureCurrentUserContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
      
        return services;
    }    
}
