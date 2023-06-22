namespace Sfinx.App.Shared.Interfaces;

public interface IStorageCache
{
    Task AddFolderItemAsync<T>(string collectionName, string id, T item);
    Task<IEnumerable<T>> GetFolderItemsAsync<T>(string collectionName);
    Task ClearFolderItemsAsync(string collectionName);
    Task<IDictionary<string, T>> GetListAsync<T>(string collectionName, bool secret, Func<Task<IDictionary<string,T>>> action, bool forceRefresh = false);
    Task<T> GetItemFromListAsync<T>(string collectionName, string id, bool secret, Func<Task<IDictionary<string,T>>> listAction, bool forceRefresh = false);
    Task<T> GetAsync<T>(string id, bool secret, Func<Task<T>> action, bool forceRefresh = false);
}