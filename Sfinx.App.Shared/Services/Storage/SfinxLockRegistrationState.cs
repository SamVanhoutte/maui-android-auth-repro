using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models.Registration;
using Sfinx.Backend.WebAPI.Client;

namespace Sfinx.App.Shared.Services.Storage;

public class SfinxLockRegistrationState
{
    private readonly IAppStorageProvider appStorageProvider;

    public SfinxLockRegistrationState(IAppStorageProvider appStorageProvider)
    {
        this.appStorageProvider = appStorageProvider;
    }


    public async Task SetOrganizationDataAsync(OrganizationRegistrationData value)
    {
        OrganizationData = value;
        await PersistToStorageAsync();
    }

    public async Task SetProductTypeAsync(Product value)
    {
        ProductType = value;
        await PersistToStorageAsync();
    }

    public async Task SetInvitationAsync(Invitation value)
    {
        LockExternalRef = value.ExternalReference;
        OrganizationId = value.OrganizationId;
        await PersistToStorageAsync();
    }

    public async Task SetInvitationCodeAsync(string value)
    {
        InvitationCode = value;
        await PersistToStorageAsync();
    }

    public string? InstanceId { get; private set; }
    private bool dataLoaded;
    public OrganizationRegistrationData? OrganizationData { get; private set; }
    public Product? ProductType { get; private set; }
    public Lock? Lock { get; private set; }
    public string? LockExternalRef { get; private set; }
    public string? InvitationCode { get; private set; }
    public string? OrganizationId { get; set; }

    public async Task EnsureDataLoadedAsync()
    {
        try
        {
            if (dataLoaded) return;
            InstanceId = await LoadPropertyAsync(nameof(InstanceId), Guid.NewGuid().ToString());
            OrganizationData = await LoadPropertyAsync<OrganizationRegistrationData?>(nameof(OrganizationData));
            ProductType = await LoadPropertyAsync<Product?>(nameof(ProductType));
            Lock = await LoadPropertyAsync<Lock?>(nameof(Lock));
            LockExternalRef = await LoadPropertyAsync<string?>(nameof(LockExternalRef)) ?? "";
            OrganizationId = await LoadPropertyAsync<string?>(nameof(OrganizationId)) ?? "";
            InvitationCode ??= await LoadPropertyAsync<string?>(nameof(InvitationCode));
            dataLoaded = true;
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task PersistToStorageAsync()
    {
        try
        {
            await appStorageProvider.SaveToStorageAsync(nameof(InstanceId), InstanceId!);
            await appStorageProvider.SaveToStorageAsync(nameof(OrganizationData), OrganizationData!);
            await appStorageProvider.SaveToStorageAsync(nameof(InvitationCode), InvitationCode!);
            await appStorageProvider.SaveToStorageAsync(nameof(LockExternalRef), LockExternalRef);
            await appStorageProvider.SaveToStorageAsync(nameof(OrganizationId), OrganizationId);
            await appStorageProvider.SaveToStorageAsync(nameof(Lock), Lock!);
            await appStorageProvider.SaveToStorageAsync(nameof(ProductType), ProductType!);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex);
            // Ignore
        }
    }

    private async Task<T?> LoadPropertyAsync<T>(string propertyName, T? defaultValue = default(T))
    {
        var result = await appStorageProvider.ReadFromStorageAsync<T>(propertyName);
        return result ?? defaultValue;
    }

    public async Task ResetAsync()
    {
        await appStorageProvider.DeleteFromStorageAsync(nameof(InstanceId));
        await appStorageProvider.DeleteFromStorageAsync(nameof(OrganizationData));
        await appStorageProvider.DeleteFromStorageAsync(nameof(InvitationCode));
        await appStorageProvider.DeleteFromStorageAsync(nameof(ProductType));
        await appStorageProvider.DeleteFromStorageAsync(nameof(Lock));
        await appStorageProvider.DeleteFromStorageAsync(nameof(LockExternalRef));
        await appStorageProvider.DeleteFromStorageAsync(nameof(OrganizationId));
        await appStorageProvider.DeleteFromStorageAsync(nameof(Invitation)); 
    }

    public async Task SetLockAsync(Lock @lock)
    {
        Lock = @lock;
        await PersistToStorageAsync();
    }
}