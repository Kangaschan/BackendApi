using Asp.Versioning;
using Asp.Versioning.Conventions;
using Asp.Versioning.Builder;

namespace LRA.Gateways.Client.Extensions;

public static class VesioningExtension
{
    public static IServiceCollection AddVersionning(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddMvc(options =>
        {
            options.Conventions.Add(new VersionByNamespaceConvention());
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        return services;
    }    
}
