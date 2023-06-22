namespace Sfinx.App.Shared.Interfaces;

public interface IAppStorageProvider
{
    Task DeleteFromStorageAsync(string key, bool secret = false);
    Task<T> ReadFromStorageAsync<T>(string key, bool secret = false);
    Task SaveToStorageAsync<T>(string key, T value, bool secret = false);
}