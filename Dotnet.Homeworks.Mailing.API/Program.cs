using Dotnet.Homeworks.Mailing.API.Configuration;
using Dotnet.Homeworks.Mailing.API.Services;
using Dotnet.Homeworks.Mailing.API.ServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));

builder.Services.AddScoped<IMailingService, MailingService>();
builder.Configuration.AddEnvironmentVariables();

var rabbitMqConfig = new RabbitMqConfig();

builder.Configuration.GetSection("RabbitMqConfig").Bind(rabbitMqConfig);
builder.Services.AddMasstransitRabbitMq(rabbitMqConfig);

var app = builder.Build();

app.Run();