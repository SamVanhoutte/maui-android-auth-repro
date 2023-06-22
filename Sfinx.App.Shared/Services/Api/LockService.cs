using Sfinx.App.Shared.Interfaces;
using Sfinx.Backend.WebAPI.Client;

namespace Sfinx.App.Shared.Services.Api;

public class VendorService : IVendorService
{
    private readonly ISfinxApiBuilder apiBuilder;
    private readonly IUserProfileManager profileManager;
    private readonly IStorageCache storageCache;
    private IVendorsApiClient? vendorsApiClient;
    public VendorService(ISfinxApiBuilder apiBuilder, IUserProfileManager profileManager, IStorageCache storageCache)
    {
        this.apiBuilder = apiBuilder;
        this.profileManager = profileManager;
        this.storageCache = storageCache;
    }
    
    private async Task<IVendorsApiClient> GetVendorApiClientAsync()
    {
        var client = await apiBuilder.GetClientAsync();
        vendorsApiClient = new VendorsApiClient(client);
        return vendorsApiClient;
    }


    public async Task<ICollection<Vendor>> GetVendorsAsync(bool forceRefresh = false)
    {
        var vendors = await storageCache.GetListAsync("vendors", false, RetrieveVendorsFromApiAsync, forceRefresh);
        return vendors.Values;
    }

    public async Task<ICollection<Product>> GetVendorProductsAsync(string vendorId, bool forceRefresh = false)
    {
        var products = await storageCache.GetListAsync($"{vendorId}-products", false, async() => await RetrieveVendorProductsFromApiAsync(vendorId), forceRefresh);
        return products.Values;
    }
    
    private async Task<IDictionary<string, Vendor>> RetrieveVendorsFromApiAsync()
    {
        var vendorsClient = await GetVendorApiClientAsync();
        var vendorResponse = await vendorsClient.GetVendorsAsync();
        return vendorResponse.Vendors
            .Where(vendor=>vendor.IsSupported)
            .ToDictionary(vendor => vendor.Id, vendor => vendor);
    }

    private async Task<IDictionary<string, Product>> RetrieveVendorProductsFromApiAsync(string vendorId)
    {
        var vendorsClient = await GetVendorApiClientAsync();
        var vendorResponse = await vendorsClient.GetVendorProductsAsync(vendorId);
        return vendorResponse.Products
            .Where(prd=>prd.IsSupported)
            .ToDictionary(prd => prd.Id, prd => prd);
    }
}
public class LockService : ILockService
{
    private readonly ISfinxApiBuilder apiBuilder;
    private readonly IUserProfileManager profileManager;
    private readonly IStorageCache storageCache;
    private IUsersApiClient? usersClient;
    private IOrganizationsApiClient? organizationsClient;
    private ILocksApiClient? locksClient;
    public LockService(ISfinxApiBuilder apiBuilder, IUserProfileManager profileManager, IStorageCache storageCache)
    {
        this.apiBuilder = apiBuilder;
        this.profileManager = profileManager;
        this.storageCache = storageCache;
    }

    private async Task<ILocksApiClient> GetLocksApiClientAsync()
    {
        var client = await apiBuilder.GetClientAsync();
        locksClient = new LocksApiClient(client);
        return locksClient;
    }
    
    private async Task<IUsersApiClient> GetUsersApiClientAsync()
    {
        var client = await apiBuilder.GetClientAsync();
        usersClient = new UsersApiClient(client);
        return usersClient;
    }

    private async Task<IOrganizationsApiClient> GetOrganizationsApiClientAsync()
    {
        var client = await apiBuilder.GetClientAsync();
        organizationsClient = new OrganizationsApiClient(client);
        return organizationsClient;
    }

    public async Task<IEnumerable<Lock>> GetLocksAsync(bool forceRefresh = false)
    {
        var locks = await storageCache.GetListAsync("locks", false, RetrieveLocksFromApiAsync, forceRefresh);
        return locks.Values;
    }

    public async Task<IEnumerable<Key>> GetKeysAsync(string? lockId = null, bool forceRefresh = false)
    {
        IDictionary<string, Key> keys = new Dictionary<string, Key>();
        if (string.IsNullOrEmpty(lockId))
        {
            keys = await storageCache.GetListAsync("keys", true, RetrieveKeysFromApiAsync, forceRefresh);
        }
        else
        {
            keys = await storageCache.GetListAsync($"keys_{lockId}", true, async() => await RetrieveKeysForLockAsync(lockId) , forceRefresh);
        }

        return keys.Values;
    }

    public async Task<OrganizationLock> GetLockByReferenceAsync(string organizationId, string lockReference,
        bool forceRefresh = false)
    {
        var @lock = await storageCache.GetAsync($"{organizationId}__{lockReference}", false,
            async () => await GetLockByRefFromApiAsync(organizationId, lockReference), forceRefresh);
        return @lock;
    }

    private async Task<IDictionary<string, Lock>> RetrieveLocksFromApiAsync()
    {
        var userId = await profileManager.GetUserIdAsync();
        var usersApiClient = await GetUsersApiClientAsync();
        var lockResponse = await usersApiClient.GetUserLocksAsync(userId);
        return lockResponse.Locks.ToDictionary(lck => lck.Id, lck => lck);
    }

    private async Task<IDictionary<string,Key>> RetrieveKeysForLockAsync(string lockId)
    {
        var locksApiClient = await GetLocksApiClientAsync();
        var userId = await profileManager.GetUserIdAsync();
        var keyResponse = await locksApiClient.GetKeysAsync(lockId);
        return keyResponse.Keys.ToDictionary(key => key.Id, key => key);
    }

    private async Task<IDictionary<string, Key>> RetrieveKeysFromApiAsync()
    {
        var usersApiClient = await GetUsersApiClientAsync();
        var userId = await profileManager.GetUserIdAsync();
        var keyResponse = await usersApiClient.GetKeysAsync(userId);
        return keyResponse.Keys.ToDictionary(key => key.Id, key => key);
    }

    private async Task<OrganizationLock?> GetLockByRefFromApiAsync(string organizationId, string lockReference)
    {
        var orgApiClient = await GetOrganizationsApiClientAsync();
        var locksResponse = await orgApiClient.FindLocksAsync(organizationId, lockReference);
        if (locksResponse.Locks.Any())
        {
            return locksResponse.Locks.First();
        }

        return null;
    }
}