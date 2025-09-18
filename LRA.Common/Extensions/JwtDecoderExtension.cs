using LRA.Common.Implementation;
using LRA.Common.Interfaces;
using LRA.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Common.Extensions;

public static class JwtDecoderExtension
{
    public static IServiceCollection AddJwtDecoder(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JsonWebKeysList>(configuration.GetSection("JsonWebKeysList"));
        services.AddTransient<IJwtTokenDecoder, JwtTokenDecoder>();
      
        return services;
    }    
}
