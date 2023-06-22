using Sfinx.Backend.WebAPI.Client;

namespace Sfinx.App.Shared.Interfaces;

public interface ILockService
{
    Task<IEnumerable<Lock>> GetLocksAsync(bool forceRefresh = false);
    Task<IEnumerable<Key>> GetKeysAsync(string? lockId = null, bool forceRefresh = false);
    

    Task<OrganizationLock?> GetLockByReferenceAsync(string organizationId, string lockReference, bool forceRefresh = false);
}