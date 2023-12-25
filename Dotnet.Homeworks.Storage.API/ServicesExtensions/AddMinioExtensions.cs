using Dotnet.Homeworks.Storage.API.Configuration;
using Minio;

namespace Dotnet.Homeworks.Storage.API.ServicesExtensions;

public static class AddMinioExtensions
{
    public static IServiceCollection AddMinioClient(this IServiceCollection services,
        MinioConfig minioConfiguration)
    {
        var minioClient = new MinioClient()
            .WithEndpoint(minioConfiguration.Endpoint, minioConfiguration.Port)
            .WithSSL(minioConfiguration.WithSsl)
            .WithCredentials(minioConfiguration.Username, minioConfiguration.Password)
            .Build();
        return services.AddSingleton<IMinioClient>(minioClient);
    }
}