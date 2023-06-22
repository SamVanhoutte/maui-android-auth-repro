namespace Sfinx.App.Shared.Interfaces;

public interface ISfinxApiBuilder
{
    Task<HttpClient?> GetClientAsync(bool enforceRefresh=true);
    Task<HttpClient> GetPublicClientAsync();

}