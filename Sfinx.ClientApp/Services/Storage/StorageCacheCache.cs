using Newtonsoft.Json;
using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Services.Extensions;

namespace Sfinx.ClientApp.Services.Storage;

public class StorageCacheCache : IStorageCache
{
    private readonly IAppStorageProvider appStorageProvider;

    public StorageCacheCache(IAppStorageProvider appStorageProvider)
    {
        this.appStorageProvider = appStorageProvider;
    }
    public async Task AddFolderItemAsync<T>(string collectionName, string key, T value)
    {
        var storedValue = JsonConvert.SerializeObject(value);
        var directoryName = Path.Combine(FileSystem.Current.CacheDirectory, collectionName);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        var cacheFile = Path.Combine(directoryName, $"{key}.json");
        await File.WriteAllTextAsync(cacheFile, storedValue);
    }

    public Task<IEnumerable<T>> GetFolderItemsAsync<T>(string collectionName)
    {
        var directoryName = Path.Combine(FileSystem.Current.CacheDirectory, collectionName);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        var items = new List<T> { };
        items.AddRange(Directory.GetFiles(directoryName, "*.json")
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<T>));
        return Task.FromResult<IEnumerable<T>>(items);
    }

    public  Task ClearFolderItemsAsync(string collectionName)
    {
        var directoryName = Path.Combine(FileSystem.Current.CacheDirectory, collectionName);
        if (Directory.Exists(directoryName))
        {
            foreach (var fileItem in Directory.GetFiles(directoryName))
            {
                File.Delete(fileItem);
            }
        }
        return Task.CompletedTask;
    }

    public async Task<IDictionary<string, T>> GetListAsync<T>(string collectionName, bool secret,
        Func<Task<IDictionary<string, T>>> listAction, bool forceRefresh = false)
    {
        var collection = await appStorageProvider.ReadFromStorageAsync<IDictionary<string, T>>(collectionName, secret);
        if (collection == null || forceRefresh)
        {
            collection = await listAction();
            await appStorageProvider.SaveToStorageAsync(collectionName, collection, secret);
        }

        return collection;
    }

    public async Task<T> GetItemFromListAsync<T>(string collectionName, string id, bool secret,
        Func<Task<IDictionary<string, T>>> listAction, bool forceRefresh = false)
    {
        var items = await GetListAsync(collectionName, secret, listAction, forceRefresh);
        return items != null ? items.GetValue(id) : default;
    }

    public async Task<T> GetAsync<T>(string id, bool secret, Func<Task<T>> action, bool forceRefresh = false)
    {
        var item = await appStorageProvider.ReadFromStorageAsync<T>(id, secret);
        if (EqualityComparer<T>.Default.Equals(item, default) || forceRefresh)
        {
            item = await action();
            await appStorageProvider.SaveToStorageAsync(id, item,secret);
        }

        return item;
    }
}