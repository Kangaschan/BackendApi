using LRA.Common.Extensions;
using LRA.Common.Middlewares;
using LRA.Subscriptions.Application.Extensions;
using LRA.Subscriptions.DBInfrastructure.Data;
using LRA.Subscriptions.DBInfrastructure.Extensions;
using LRA.Subscriptions.StripeInfrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSerilog(builder.Configuration);
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureStripeSetting(builder.Configuration);
builder.Services.ConfigureStripeServices(builder.Configuration);
builder.Services.ConfigureRepositories(builder.Configuration);
builder.Services.ConfigureApiKeys(builder.Configuration);
builder.Services.AddKafkaConsumer(builder.Configuration);

builder.Services.ConfigureServices(builder.Configuration);
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
    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
    {
        Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations:");
        foreach (var migration in pendingMigrations)
        {
            Console.WriteLine($"- {migration}");
        }

        Console.WriteLine("Migrations applied successfully.");
    }
    else
    {
        Console.WriteLine("No pending migrations to apply.");
    }
}

app.Run();
