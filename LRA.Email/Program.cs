using LRA.Email.Extensions;
using static LRA.Common.Extensions.SerilogExtension;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureSerilog(builder.Configuration);
builder.Services.ConfigureSmtp(builder.Configuration);
builder.Services.AddKafkaConsumer(builder.Configuration);

var app = builder.Build();

app.Run();
