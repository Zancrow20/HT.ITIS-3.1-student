using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;

namespace Dotnet.Homeworks.Storage.API.Services;

public class StorageFactory : IStorageFactory
{
    private readonly IMinioClient _minioClient;

    public StorageFactory(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<IStorage<Image>> CreateImageStorageWithinBucketAsync(string bucketName)
    {
        var bucketExists = new BucketExistsArgs().WithBucket(bucketName);
        var found = await _minioClient.BucketExistsAsync(bucketExists);
        if (found)
            return new ImageStorage(_minioClient, bucketName);

        var bucketMake = new MakeBucketArgs().WithBucket(bucketName);
        await _minioClient.MakeBucketAsync(bucketMake);
        return new ImageStorage(_minioClient, bucketName);
    }
}