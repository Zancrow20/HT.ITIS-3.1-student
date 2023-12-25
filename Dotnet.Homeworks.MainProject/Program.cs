using Dotnet.Homeworks.Features;
using Dotnet.Homeworks.MainProject.BackgroundServices;
using Dotnet.Homeworks.MainProject.Configuration;
using Dotnet.Homeworks.MainProject.ServicesExtensions.ApplicationServices;
using Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();


builder.Configuration.AddEnvironmentVariables();

var rabbitMqConfig = new RabbitMqConfig();

builder.Configuration.GetSection("RabbitMqConfig").Bind(rabbitMqConfig);

builder.Services
    .AddMasstransitRabbitMq(rabbitMqConfig)
    .AddApplicationServices(builder.Configuration)
    .AddHostedService<DbContextMigration>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();