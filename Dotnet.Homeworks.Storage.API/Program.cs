using Dotnet.Homeworks.Storage.API.Configuration;
using Dotnet.Homeworks.Storage.API.Endpoints;
using Dotnet.Homeworks.Storage.API.Services;
using Dotnet.Homeworks.Storage.API.ServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

var minioConfig = new MinioConfig();

builder.Configuration.GetSection("MinioConfig").Bind(minioConfig);

builder.Services.Configure<MinioConfig>(builder.Configuration.GetSection("MinioConfig"));
builder.Services.AddSingleton<IStorageFactory, StorageFactory>();
builder.Services.AddHostedService<PendingObjectsProcessor>();
builder.Services.AddMinioClient(minioConfig);

var app = builder.Build();

app.MapProductsEndpoints();

app.Run();