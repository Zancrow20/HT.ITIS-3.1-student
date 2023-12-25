using System.Reactive.Linq;
using Dotnet.Homeworks.Shared.Dto;
using Dotnet.Homeworks.Storage.API.Constants;
using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;

namespace Dotnet.Homeworks.Storage.API.Services;

public class ImageStorage : IStorage<Image>
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public ImageStorage(IMinioClient minioClient, string bucketName)
    {
        _minioClient = minioClient;
        _bucketName = bucketName;
    }

    public async Task<Result> PutItemAsync(Image item, CancellationToken cancellationToken = default)
    {
        try
        {
            if (item.Content == null) throw new ArgumentNullException(nameof(item.Content));
            var beArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            var found = await _minioClient.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(mbArgs, cancellationToken).ConfigureAwait(false);
            }

            item.Metadata[MetadataKeys.Destination] = _bucketName;
            var putObj = new PutObjectArgs()
                .WithBucket(Buckets.Pending)
                .WithObject(item.FileName)
                .WithContentType(item.ContentType)
                .WithStreamData(item.Content)
                .WithObjectSize(item.Content.Length)
                .WithHeaders(item.Metadata);
            
            await _minioClient.PutObjectAsync(putObj, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<Image?> GetItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            var found = await _minioClient.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false);
            if (!found)
                return null;

            Stream itemStream = new MemoryStream();
            var getObj = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(itemName)
                .WithCallbackStream(stream => stream.CopyToAsync(itemStream, cancellationToken));
            
            var obj = await _minioClient.GetObjectAsync(getObj, cancellationToken);
            itemStream.Position = 0;
            
            return new Image(itemStream, obj.ObjectName, obj.ContentType, obj.MetaData);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<Result> RemoveItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            var found = await _minioClient.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false);
            if (!found)
                return false;

            var removeObj = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(itemName);

            await _minioClient.RemoveObjectAsync(removeObj, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<IEnumerable<string>> EnumerateItemNamesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            var found = await _minioClient.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false);
            if (!found)
                return Enumerable.Empty<string>();

            var args = new ListObjectsArgs()
                .WithBucket(_bucketName);

            var observable = await _minioClient.ListObjectsAsync(args, cancellationToken)
                .Select(o => o.Key).ToList();
            return observable;
        }
        catch (Exception e)
        {
            return new []{e.Message};
        }
    }

    public async Task<Result> CopyItemToBucketAsync(string itemName, string destinationBucketName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            var found = await _minioClient.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false);
            if (!found)
                return false;
            
            var destBucketArgs = new BucketExistsArgs()
                .WithBucket(destinationBucketName);
            found = await _minioClient.BucketExistsAsync(destBucketArgs, cancellationToken).ConfigureAwait(false);
            if (!found)
                return false;

            var copySrc = new CopySourceObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(itemName);

            var copyDest = new CopyObjectArgs()
                .WithBucket(destinationBucketName)
                .WithObject(itemName)
                .WithCopyObjectSource(copySrc);

            await _minioClient.CopyObjectAsync(copyDest, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}