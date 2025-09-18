using LRA.Account.Application.Extensions;
using LRA.Account.DBInfrastructure.Data;
using LRA.Account.DBInfrastructure.Extensions;
using LRA.Account.KeycloakInfrastructure.Extensions;
using LRA.Common.Middlewares;
using LRA.Infrastructure.Azure.Extensions;
using LRA.Infrastructure.Messaging.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using static LRA.Common.Extensions.ApiKeysExtension;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAzureService(builder.Configuration);
builder.Services.AddFileService(builder.Configuration);
builder.Services.AddProducer(builder.Configuration.GetSection("Kafka:Email-send"));
builder.Services.AddSendMailService(builder.Configuration);
builder.Services.AddKeycloakClient(builder.Configuration);
builder.Services.ConfigureDb(builder.Configuration);
builder.Services.ConfigureRepositories(builder.Configuration);
builder.Services.ConfigureApiKeys(builder.Configuration);
builder.Services.ConfigureAccountService(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LRA API", Version = "v1" });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key must be provided in the header",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Scheme = "ApiKey"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
