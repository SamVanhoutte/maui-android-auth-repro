using Microsoft.Extensions.Options;
using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models.Configuration;

namespace Sfinx.App.Shared.Services;

public abstract class SfinxApiBuilder : ISfinxApiBuilder
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IUserProfileManager userProfileManager;

    public SfinxApiBuilder(IHttpClientFactory httpClientFactory,
        IUserProfileManager userProfileManager)
    {
        this.httpClientFactory = httpClientFactory;
        this.userProfileManager = userProfileManager;
    }

    protected abstract Task<ApiEnvironmentSetting?> LoadEnvironmentSettingsAsync();

    public async Task<HttpClient?> GetClientAsync(bool enforceRefresh = true)
    {
        var client = httpClientFactory.CreateClient();
        var environmentSetting = await LoadEnvironmentSettingsAsync();
        if (environmentSetting != null)
        {
            var bearerToken = await userProfileManager.GetBearerTokenAsync(enforceRefresh);
            if (string.IsNullOrEmpty(bearerToken)) return null;
            client.BaseAddress = new Uri(environmentSetting.BaseUri);
#if DEBUG
            var userId = await userProfileManager.GetUserIdAsync();
            client.DefaultRequestHeaders.Add("sfx-user-id", userId);
            client.DefaultRequestHeaders.Add("sfx-client-id", userId);
#endif
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            client.DefaultRequestHeaders.Add("sfx-subscription-key", environmentSetting.SubscriptionKey);
        }

        return client;
    }

    public async Task<HttpClient> GetPublicClientAsync()
    {
        var client = httpClientFactory.CreateClient();
        var environmentSetting = await LoadEnvironmentSettingsAsync();
        if (environmentSetting != null)
        {
            client.BaseAddress = new Uri(environmentSetting.PublicUri);
        }

        return client;
    }
}