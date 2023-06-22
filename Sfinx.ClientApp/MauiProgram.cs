using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Maui.LifecycleEvents;
using MudBlazor.Services;
using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models.Configuration;
using Sfinx.App.Shared.Services;
using Sfinx.App.Shared.Services.MultiLinguality;
using Sfinx.ClientApp.Services.Security;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sfinx.App.Shared.Services.Api;
using Sfinx.App.Shared.Services.ErrorHandling;
using Sfinx.App.Shared.Services.Storage;
using Sfinx.ClientApp.Services;
using Sfinx.ClientApp.Services.Storage;
using Microsoft.Extensions.Logging;
using Sfinx.ClientApp.Services.DeepLink;
using Sfinx.ClientApp.Services.Tracing;

namespace Sfinx.ClientApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureLifecycleEvents(events =>
            {
#if IOS
                events.AddiOS(platform =>
                {
                    platform.OpenUrl((app, url, options) =>
                        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url));
                });
#elif ANDROID
                events.AddAndroid(platform =>
                {
                    platform.OnActivityResult(((activity, code, resultCode, data) =>
                    {
                        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(code, resultCode, data);
                    }));
                });

#endif
            })
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        var a = Assembly.GetExecutingAssembly();

        var resources = typeof(MauiProgram).Assembly.GetManifestResourceNames();

        using var appSettingsStream = a.GetManifestResourceStream("Sfinx.ClientApp.appsettings.json");
        using var appSettingsDevStream = a.GetManifestResourceStream("Sfinx.ClientApp.appsettings.Development.json");


        var cfgBuilder = new ConfigurationBuilder();
        if (appSettingsStream != null) cfgBuilder.AddJsonStream(appSettingsStream);
        if (appSettingsDevStream != null) cfgBuilder.AddJsonStream(appSettingsDevStream);

        var config = cfgBuilder.Build();

        builder.Configuration.AddConfiguration(config);
        builder.Services.Configure<ApiSettings>(options =>
            builder.Configuration.GetSection("SfinxBackendApi").Bind(options));
        // builder.Services.Configure<AzureB2CSettings>(options =>
        //     builder.Configuration.GetSection("AzureAdB2C").Bind(options));
        builder.Services.Configure<PortalSettings>(options =>
            builder.Configuration.GetSection("SfinxPortal").Bind(options));

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AppAuthenticationManager>();
        builder.Services.AddSingleton<DeeplinkAppService>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
            provider.GetRequiredService<AppAuthenticationManager>());
        // builder.Services.AddSingleton<AuthenticatedUser>();
        builder.Services.AddMudServices();
        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif
        builder.Services.AddInheritStringLocalizerFactory();


        builder.Services.AddSingleton<IThemeManager, SfinxThemeManager>();
        builder.Services.AddSingleton(sp => new HttpClient() { });
        builder.Services.AddSingleton<IStorageCache, StorageCacheCache>();
        builder.Services.AddSingleton<ICrashReportHandler, AppCrashReportManager>();
        builder.Services.AddSingleton<AppTracer>();
        builder.Services.AddScoped<ITokenStorage, SecureTokenStorage>();
        builder.Services.AddScoped<IAppStorageProvider, MobileAppStorageProvider>();
        builder.Services.AddScoped<ISfinxApiBuilder, SfinxAppApiBuilder>();
        builder.Services.AddScoped<IAuthenticationManager, AppAuthenticationManager>();
        builder.Services.AddScoped<IUserProfileManager, SfinxAdB2CProfileManager>();
        builder.Services.AddScoped<IErrorHandler, ApiErrorHandler>();
        builder.Services.AddScoped<SfinxLockRegistrationState>();
        builder.Services.AddSingleton<ILockService, LockService>();
        builder.Services.AddSingleton<IVendorService, VendorService>();
        builder.Services.AddSingleton<IAlertNotifier, AppAlertNotifier>();

        builder.Services.AddHttpClient();

        builder.Services.AddSingleton<TranslatorService>();
        builder.Services.AddLocalization(options => { options.ResourcesPath = "Languages"; });

        builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
        builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        try
        {
            // builder.Services.AddSingleton<ILoggerProvider>(p =>
            //     new UnhandledExceptionProvider(
            //         p.GetService<IUnhandledExceptionProcessor>(),
            //         p.GetService<ICrashReportHandler>())
            // );
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        var host = builder.Build();

        return host;
    }
}