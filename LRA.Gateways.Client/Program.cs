using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using LRA.Common.Extensions;
using LRA.Common.Validators;
using LRA.Gateways.Client.Extensions;
using LRA.Gateways.Client.Implementations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureAccountService(builder.Configuration);
builder.Services.ConfigureSubscriptionsService(builder.Configuration);
builder.Services.AddJwtDecoder(builder.Configuration);
builder.Services.AddVersionning(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Client API", Version = "v1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "Client API", Version = "v2" });
    
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo))
            return false;

        var versions = apiDesc.ActionDescriptor.EndpointMetadata
            .OfType<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions);

        return versions.Any(v => $"v{v.MajorVersion}" == docName);
    });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and the JWT token. Example: `Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...`"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAuthentication("JwtAuthentication")
    .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>("JwtAuthentication", null);
builder.Services.AddValidatorsFromAssemblyContaining<RegisterAccountRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TokenValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CompleteResetPasswordRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangeEmailRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Client API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Client API v2");

        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
