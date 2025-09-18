using LRA.Subscriptions.Application.Interfaces;
using LRA.Subscriptions.DBInfrastructure.Data;
using LRA.Subscriptions.DBInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Subscriptions.DBInfrastructure.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
    
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services,IConfiguration configuration)
    {
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ISubscriptionsRepository,SubscriptionsRepository>();

        return services;
    } 
}
