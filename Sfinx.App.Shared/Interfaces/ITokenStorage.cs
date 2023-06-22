using Sfinx.App.Shared.Models.Security;

namespace Sfinx.App.Shared.Interfaces;

public interface ITokenStorage
{
    Task<IEnumerable<AuthToken>?> GetTokensAsync();
    Task ClearTokensAsync();
    Task PersistTokensAsync(IEnumerable<AuthToken> tokens);
}