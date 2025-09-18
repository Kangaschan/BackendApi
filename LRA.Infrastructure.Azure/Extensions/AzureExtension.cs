using Azure.Storage.Blobs;
using LRA.Infrastructure.Azure.Implementations;
using LRA.Infrastructure.Azure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LRA.Infrastructure.Azure.Extensions;

public static class AzureExtension
{
    public static IServiceCollection AddAzureService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ => 
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            return new BlobServiceClient(connectionString);
        });
        
        services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
        return services;
    }
}
