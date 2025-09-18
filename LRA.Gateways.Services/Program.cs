using LRA.Gateways.Services.Implementaions;
using LRA.Gateways.Services.Interfaces;
using LRA.Infrastructure.Messaging.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IStripeWebHookService, StripeWebHookService>();
builder.Services.AddProducer(builder.Configuration.GetSection("Kafka:Subscriptions"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Subscriptions API", 
        Version = "v1",
        Description = "Subscriptions API"
    });
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Subscriptions API v1");
    });
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
