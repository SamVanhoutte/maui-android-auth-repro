using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models.Organizations;
using Sfinx.App.Shared.Models.Security;
using Sfinx.App.Shared.Services.Extensions;

namespace Sfinx.App.Shared.Services
{
    public class SfinxAdB2CProfileManager : IUserProfileManager
    {
        private readonly IAuthenticationManager authenticationManager;

        public SfinxAdB2CProfileManager(IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }

        public async Task<string?> GetEmailAsync()
        {
            return await GetClaimAsync("emails", true);
        }

        public async Task<string?> GetUserIdAsync()
        {
            var uidClaim = "extension_SfinxUserId";
            return await GetClaimAsync(uidClaim, true);
        }

        public async Task<bool> IsAdminAsync()
        {
            var email = await GetEmailAsync();
            return email?.EndsWith("@sfinxinside.com", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public async Task<bool> IsOrganizationAdminAsync()
        {
            if (await IsAdminAsync()) return true;
            var orgs = await GetOrganizationMembershipsAsync();
            return orgs.Any(o=> o.Role == "Administrator");
        }

        public async Task<List<OrganizationMembership>> GetOrganizationMembershipsAsync()
        {
            var orgClaim = await GetClaimAsync("extension_SfinxOrganizationMemberships", true);
            if (string.IsNullOrEmpty(orgClaim)) return new List<OrganizationMembership>();

            var orgs = JsonConvert.DeserializeObject<OrganizationMembership[]>(orgClaim);
            return orgs.ToList();
        }


        public async Task<string?> GetFullNameAsync()
        {
            return await GetClaimAsync("name", true);
        }


        private async Task<string?> GetClaimAsync(string claimName, bool checkJwtTokenWhenEmpty)
        {
            var claims = await GetClaimsAsync();
            if (!claims.ContainsKey(claimName) && checkJwtTokenWhenEmpty)
            {
                //claims = await GetClaimsFromBearerAsync();
            }

            return claims.GetValue(claimName);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            return await authenticationManager.IsAuthenticatedAsync();
        }

        public async Task SignOutAsync()
        {
            await authenticationManager.SignOutAsync();
        }

        public async Task<ClaimsPrincipal?> GetClaimsPrincipalAsync()
        {
            return await authenticationManager.GetClaimsPrincipalAsync();
        }

        public async Task<string?> GetBearerTokenAsync(bool enforceRefresh = false)
        {
            return await authenticationManager.GetBearerTokenAsync(enforceRefresh);
        }

        public async Task<IEnumerable<AuthToken>?> RefreshTokensAsync()
        {
            return await authenticationManager.RefreshTokensAsync();
        }

        public async Task<IDictionary<string, string>> GetClaimsAsync()
        {
            return await authenticationManager.GetClaimsAsync();
        }

        public async Task<DateTime> GetTokenExpirationAsync()
        {
            return await authenticationManager.GetTokenExpirationAsync();
        }


    }
}