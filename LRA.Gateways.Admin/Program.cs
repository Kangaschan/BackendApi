using FluentValidation;
using FluentValidation.AspNetCore;
using LRA.Common.Extensions;
using LRA.Common.Validators;
using LRA.Gateways.Admin.Extensions;
using LRA.Gateways.Admin.Implementations;
using LRA.Gateways.Admin.Validators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureCurrentUserContext(builder.Configuration);
builder.Services.ConfigureAccountService(builder.Configuration);
builder.Services.AddJwtDecoder(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin API" });
    
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
builder.Services.AddValidatorsFromAssemblyContaining<TokenValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CompleteResetPasswordRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangeEmailRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<JobApplicationProcessDtoValidator>();

builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin API");

        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
