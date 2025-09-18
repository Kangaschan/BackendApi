using LRA.Account.Application.Interfaces;
using LRA.Account.DBInfrastructure.Data;
using LRA.Account.DBInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Account.DBInfrastructure.Extensions;

public static class DbExtension
{
    public static IServiceCollection ConfigureDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
    
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<ITokenRepository, TokenRepository>();
        services.AddTransient<IOneTimePasswordRepository, OneTimePasswordRepository>();
        services.AddTransient<IKycRepository, KycRepository>();
        return services;
    }
}
