using LRA.Account.Application.Configuration;
using LRA.Account.Application.Implementation;
using LRA.Account.Application.Interfaces;
using LRA.Infrastructure.Messaging.Configuration;
using LRA.Infrastructure.Messaging.Extensions;
using LRA.Infrastructure.Messaging.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Account.Application.Extensions;

public static class AccountServiceExtension
{
    public static IServiceCollection ConfigureAccountService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TemporaryPasswordSettings>(
            configuration.GetSection("TemporaryPasswordSettings"));
        services.AddTransient<IAccountService, AccountService>();

        return services;
    }
    
    public static IServiceCollection AddSendMailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMailSendService, MailSendService>();
        return services;
    }
    
    public static IServiceCollection AddFileService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IAccountImageService, AccountImageService>();
        
        return services;
    }
}
