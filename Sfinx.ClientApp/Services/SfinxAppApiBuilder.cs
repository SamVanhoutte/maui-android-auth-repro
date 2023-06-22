using Microsoft.Extensions.Options;
using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models.Configuration;
using Sfinx.App.Shared.Services;

namespace Sfinx.ClientApp.Services;

public class SfinxAppApiBuilder : SfinxApiBuilder
{
    private readonly ApiSettings apiSettings;
    private readonly IAppStorageProvider appStorageProvider;

    public SfinxAppApiBuilder(IHttpClientFactory httpClientFactory,
        IUserProfileManager userProfileManager, IAppStorageProvider appStorageProvider,
        IOptions<ApiSettings> apiConfig): base(httpClientFactory, userProfileManager)
    {
        this.appStorageProvider = appStorageProvider;
        apiSettings = apiConfig.Value;
    }
    
    protected override async Task<ApiEnvironmentSetting?> LoadEnvironmentSettingsAsync()
    {
        var environment = await appStorageProvider.ReadFromStorageAsync<string>("environment") ?? "prd";
        if (apiSettings.EnvironmentSettings.TryGetValue(environment, out var environmentSetting))
        {
            return environmentSetting;
        }

        return null;
    }

}