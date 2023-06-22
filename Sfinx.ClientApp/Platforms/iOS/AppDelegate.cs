using Foundation;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Client;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Sfinx.ClientApp.Services.DeepLink;
using UIKit;

namespace Sfinx.ClientApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    // private readonly NavigationManager navigationManager;
    //
    // public AppDelegate(NavigationManager navigationManager)
    // {
    //     this.navigationManager = navigationManager;
    // }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    private string myScheme = "sfinxinside";

    [Export("application:openURL:options:")]
    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
        if (url?.Scheme?.Equals(myScheme, StringComparison.InvariantCultureIgnoreCase) ?? false)
        {
            var pageUrl = url.ToString().Replace($"{myScheme}://app", "");
            
            var appServices = ServiceHelper.Current.GetRequiredService<DeeplinkAppService>(); // MAUI cross-platform service resolver: https://stackoverflow.com/a/73521158/10388359
            appServices.OnAppLinkReceived(pageUrl);
        }

        return base.OpenUrl(application, url, options);
    }
}