using LRA.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LRA.Common.Extensions;

public static class SerilogExtension
{
    public static IServiceCollection ConfigureSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        var serilogSettings = configuration.GetSection("SerilogSettings").Get<SerilogConfiguration>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(serilogSettings.MinimumLevel)
            .WriteTo.Console(outputTemplate: serilogSettings.ConsoleOutputTemplate)
            .WriteTo.File(
                serilogSettings.LogFilePath,
                rollingInterval: serilogSettings.RollingInterval,
                outputTemplate: serilogSettings.ConsoleOutputTemplate)
            .Enrich.WithProperty("Service", serilogSettings.ServiceName)
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog(dispose: true);
        });

        return services;
    }
}
