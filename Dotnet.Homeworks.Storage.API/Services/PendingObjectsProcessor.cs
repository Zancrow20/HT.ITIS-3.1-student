using Dotnet.Homeworks.Storage.API.Constants;
using Dotnet.Homeworks.Storage.API.Dto.Internal;

namespace Dotnet.Homeworks.Storage.API.Services;

public class PendingObjectsProcessor : BackgroundService
{
    private IStorage<Image> _storage;
    private readonly IStorageFactory _storageFactory;
    private readonly PeriodicTimer _periodicTimer;

    public PendingObjectsProcessor(IStorageFactory storageFactory)
    {
        _storage = null!;
        _storageFactory = storageFactory;
        _periodicTimer = new PeriodicTimer(PendingObjectProcessor.Period);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _storage = await _storageFactory.CreateImageStorageWithinBucketAsync(Buckets.Pending);
            
            while (await _periodicTimer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                var listOfKeys = await _storage.EnumerateItemNamesAsync(stoppingToken);
                foreach (var key in listOfKeys)
                {
                    var item = await _storage.GetItemAsync(key, stoppingToken);
                    if(item is null)
                        continue;
                    var bucketExists = item.Metadata.TryGetValue(MetadataKeys.Destination, out var bucketName);
                    if (bucketExists)
                        await _storage.CopyItemToBucketAsync(key, bucketName!, cancellationToken: stoppingToken);
                    await _storage.RemoveItemAsync(key, stoppingToken);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}