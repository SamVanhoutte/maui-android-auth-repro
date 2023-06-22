using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models.Configuration;
using Sfinx.App.Shared.Models.Security;
using Sfinx.ClientApp.Services.Tracing;

namespace Sfinx.ClientApp.Services.Security;

public class AppAuthenticationManager : AuthenticationStateProvider, IAuthenticationManager
{
    private IPublicClientApplication applicationAuthClient;
    private AzureB2CSettings activeB2cSettings;
    private ITokenStorage tokenStorage;
    private readonly IAppStorageProvider appStorageProvider;
    private readonly NavigationManager navigationManager;
    private readonly AppTracer appTracer;
    private readonly ApiSettings environmentSettings;
    private ClaimsPrincipal? claimsPrincipal;

    public AppAuthenticationManager(ITokenStorage tokenStorage, IAppStorageProvider appStorageProvider,
        IOptions<ApiSettings> apiSettings, NavigationManager navigationManager, AppTracer appTracer)
    {
        this.tokenStorage = tokenStorage;
        this.appStorageProvider = appStorageProvider;
        this.navigationManager = navigationManager;
        this.appTracer = appTracer;
        if (apiSettings?.Value != null)
        {
            environmentSettings = apiSettings.Value;
        }
    }

    private async Task<(IPublicClientApplication Client, AzureB2CSettings Settings)> GetAuthSettingsAsync()
    {
        var environment = await appStorageProvider.ReadFromStorageAsync<string>("environment") ?? "prd";
        appTracer.Trace($"Current environment: {environment}");
        if (environmentSettings.EnvironmentSettings.TryGetValue(environment, out var environmentSetting))
        {
            activeB2cSettings = environmentSetting.AuthSettings;
        }


        if (applicationAuthClient?.AppConfig?.ClientId != activeB2cSettings.ClientId)
        {
            applicationAuthClient = PublicClientApplicationBuilder.Create(activeB2cSettings.ClientId)
                .WithB2CAuthority(activeB2cSettings.AuthoritySignIn) // uncomment to support B2C
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")

#if WINDOWS
            .WithRedirectUri("http://localhost")
#else
                .WithRedirectUri($"msal{activeB2cSettings.ClientId}://auth")
#endif
                .Build();
        }

        if (applicationAuthClient == null || activeB2cSettings == null)
        {
            throw new Exception("Environment settings seem to be null");
        }

        return new(applicationAuthClient, activeB2cSettings);
    }

    public async Task<string> GetBearerTokenAsync(bool enforceRefresh = false)
    {
        try
        {
            var authResult = await LoginAsync();
            appTracer.Trace($"After login, returning bearer token now");

            return authResult.AccessToken;
        }
        catch (MsalClientException mse) when (mse.ErrorCode.Equals("authentication_canceled"))
        {
            return null;
        }
    }

    public async Task<IEnumerable<AuthToken>> RefreshTokensAsync()
    {
        try
        {
            var result = await LoginAsync();
        }
        catch (MsalClientException mse) when (mse.ErrorCode.Equals("authentication_canceled"))
        {
            appTracer.Trace($"Error: {mse.Message}");
            return null;
        }

        appTracer.Trace($"After login, getting tokens now");

        return await tokenStorage.GetTokensAsync();
    }

    public async Task<IDictionary<string, string>> GetClaimsAsync()
    {
        try
        {
            var authResult = await LoginAsync();
            appTracer.Trace($"After login, getting claims now");
            return authResult.ClaimsPrincipal.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
        }
        catch (MsalClientException mse) when (mse.ErrorCode.Equals("authentication_canceled"))
        {
            appTracer.Trace($"Error: {mse.Message}");
            return new Dictionary<string, string>();
        }
    }

    public Task<DateTime> GetTokenExpirationAsync()
    {
        throw new NotImplementedException("Token expiration is not implemented for the App, only in the web");
    }

    public async Task SignOutAsync(bool redirect = false)
    {
        appTracer.Trace($"Sign out called");

        var loggedOnAccount = await GetUserAsync();
        if (loggedOnAccount != null)
        {
            appTracer.Trace($"Logged on account found");
            var authSettings = await GetAuthSettingsAsync();
            appTracer.Trace($"Removing account found");
            await authSettings.Client.RemoveAsync(loggedOnAccount);
            appTracer.Trace($"Clearing tokens");
            await tokenStorage.ClearTokensAsync();
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        if (redirect)
        {
            navigationManager.NavigateTo("/", true);
        }
    }

    public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync()
    {
        return claimsPrincipal;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var account = await GetUserAsync();
        return account != null;
    }

    private async Task<IAccount> GetUserAsync()
    {
        appTracer.Trace($"Getting user");

        var authSettings = await GetAuthSettingsAsync();

        var loggedOnAccounts = await authSettings.Client.GetAccountsAsync();
        var account = loggedOnAccounts.FirstOrDefault();
        if (account == null)
        {
            appTracer.Trace($"No account found");
        }
        else
        {
            appTracer.Trace($"Account found: {account?.Username ?? "NULL"}");
        }

        return account;
    }

    private async Task<AuthenticationResult> LoginAsync()
    {
try
{
    AuthenticationResult authResult = null;
    var tokens = new List<AuthToken> { };
    var loggedOnAccount = await GetUserAsync();
    if (loggedOnAccount != null)
    {
        try
        {
            appTracer.Trace($"Logged on Account found: {loggedOnAccount.Username ?? "NULL"}");

            var authSettings = await GetAuthSettingsAsync();
            authResult = await authSettings.Client
                .AcquireTokenSilent(authSettings.Settings.Scopes, loggedOnAccount)
                .WithForceRefresh(true).ExecuteAsync();
            appTracer.Trace(
                $"Auth result: {!string.IsNullOrEmpty(authResult.AccessToken)} - {authResult.IdToken}");
            tokens.Add(new AuthToken("TokenRetrieval", "Refreshed"));
        }
        catch (Exception e)
        {
            appTracer.Trace($"Error : {e.Message}");
            await SignOutAsync();
            await tokenStorage.ClearTokensAsync();
            loggedOnAccount = null;
        }
    }

    if (loggedOnAccount == null)
    {
        appTracer.Trace($"No account found , asking sign on");
        var authSettings = await GetAuthSettingsAsync();
#if ANDROID
        appTracer.Trace($"Android, so WithParent should be called");
#endif

        try
        {
            authResult = await authSettings.Client
                .AcquireTokenInteractive(authSettings.Settings.Scopes)
                .WithPrompt(Prompt.ForceLogin)
#if ANDROID
            .WithParentActivityOrWindow(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity)
#endif
#if WINDOWS
.WithUseEmbeddedWebView(false)
#endif
                .ExecuteAsync();

        }
        catch (Exception e)
        {
            appTracer.Trace(e.ToString());
            throw;
        }
        appTracer.Trace(
            $"Auth result: {(authResult?.AccessToken)?[5..] ?? "NULL"} - {authResult?.IdToken?[5..] ?? "NULL"}");
        tokens.Add(new AuthToken("TokenRetrieval", "Logged on"));
    }


    if (authResult != null)
    {
        if (!string.IsNullOrEmpty(authResult.AccessToken))
        {
            tokens.Add(new AuthToken("AccessToken", authResult.AccessToken));
        }

        claimsPrincipal = authResult.ClaimsPrincipal;
        appTracer.Trace($"Persisting tokens");

        await tokenStorage.PersistTokensAsync(tokens);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    return authResult;
}
catch (Exception e)
{
    appTracer.Trace(
        $"An error occurred during login: {e.Message} - Active B2C Settings: {activeB2cSettings?.ClientId} - Client : {applicationAuthClient?.AppConfig?.ClientId}");
    Console.WriteLine(
        $"An error occurred during login: {e.Message} - Active B2C Settings: {activeB2cSettings?.ClientId} - Client : {applicationAuthClient?.AppConfig?.ClientId}");
    throw;
}
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        appTracer.Trace($"Getting authentication state");
        if (claimsPrincipal != null)
        {
            appTracer.Trace($"Return claimsprincipal");
            return new AuthenticationState(claimsPrincipal);
        }

        var storedTokens = await tokenStorage.GetTokensAsync();
        if (storedTokens != null && storedTokens.Any())
        {
            appTracer.Trace($"Building claimsprincipal");
            var claims = storedTokens.Select(token => new Claim(token.Name, token.Value));
            var identity = new ClaimsIdentity(claims, "Custom authentication");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        return new AuthenticationState(new ClaimsPrincipal());
    }
}