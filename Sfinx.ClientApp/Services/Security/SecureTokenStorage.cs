using Newtonsoft.Json;
using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models.Security;

namespace Sfinx.ClientApp.Services.Security;

public class SecureTokenStorage : ITokenStorage
{
    private const string TokenKey = "ApiTokens";
    public async Task<IEnumerable<AuthToken>> GetTokensAsync()
    {
        var tokenValue = await SecureStorage.Default.GetAsync(TokenKey);
        return string.IsNullOrEmpty(tokenValue) ? 
            new List<AuthToken> { } : 
            JsonConvert.DeserializeObject<List<AuthToken>>(tokenValue);
    }

    public Task ClearTokensAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public async Task PersistTokensAsync(IEnumerable<AuthToken> tokens)
    {
        var tokenValue = JsonConvert.SerializeObject(tokens);
        await SecureStorage.Default.SetAsync(TokenKey, tokenValue);
    }
}