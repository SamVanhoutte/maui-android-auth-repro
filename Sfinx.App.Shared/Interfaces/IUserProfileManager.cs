using System.Security.Claims;
using Sfinx.App.Shared.Models.Organizations;
using Sfinx.App.Shared.Models.Security;

namespace Sfinx.App.Shared.Interfaces;

public interface IAuthenticationManager
{
    Task<ClaimsPrincipal?> GetClaimsPrincipalAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetBearerTokenAsync(bool enforceRefresh = false);
    Task<IEnumerable<AuthToken>?> RefreshTokensAsync();
    Task<IDictionary<string, string>> GetClaimsAsync();
    Task<DateTime> GetTokenExpirationAsync();
    Task SignOutAsync(bool redirect = false);
}

public interface IUserProfileManager
{
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetBearerTokenAsync(bool enforceRefresh = false);
    Task<IEnumerable<AuthToken>?> RefreshTokensAsync();
    Task<IDictionary<string, string>> GetClaimsAsync();
    Task<DateTime> GetTokenExpirationAsync();
    Task<string?> GetEmailAsync();
    Task<string?> GetUserIdAsync();
    Task<bool> IsAdminAsync();
    Task<bool> IsOrganizationAdminAsync();
    Task<List<OrganizationMembership>> GetOrganizationMembershipsAsync();
    Task<string?> GetFullNameAsync();
    Task SignOutAsync();
    Task<ClaimsPrincipal?> GetClaimsPrincipalAsync();
}