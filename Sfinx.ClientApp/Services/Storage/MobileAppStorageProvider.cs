using Newtonsoft.Json;
using Sfinx.App.Shared.Interfaces;

namespace Sfinx.ClientApp.Services.Storage;

public class MobileAppStorageProvider : IAppStorageProvider
{
    public async Task DeleteFromStorageAsync(string key, bool secret = false)
    {
        if (secret)
        {
            SecureStorage.Default.Remove(key);
        }
        else
        {
            var cacheFile = Path.Combine(FileSystem.Current.CacheDirectory, $"{key}.json");
            if (File.Exists(cacheFile))
            {
                File.Delete(cacheFile);
            }
        }
    }

    public async Task<T> ReadFromStorageAsync<T>(string key, bool secret = false)
    {
        string storedValue = null;
        if (secret)
        {
            storedValue = await SecureStorage.Default.GetAsync(key);
        }
        else
        {
            var cacheFile = Path.Combine(FileSystem.Current.CacheDirectory, $"{key}.json");
            if (File.Exists(cacheFile))
            {
                storedValue = await File.ReadAllTextAsync(cacheFile);
            }
        }

        if (string.IsNullOrEmpty(storedValue)) return default;
        return JsonConvert.DeserializeObject<T>(storedValue);
    }

    public async Task SaveToStorageAsync<T>(string key, T value, bool secret = false)
    {
        var storedValue = JsonConvert.SerializeObject(value);
        if (secret)
        {
            await SecureStorage.Default.SetAsync(key, storedValue);
        }
        else
        {
            var cacheFile = Path.Combine(FileSystem.Current.CacheDirectory, $"{key}.json");
            await File.WriteAllTextAsync(cacheFile, storedValue);
        }
    }
}