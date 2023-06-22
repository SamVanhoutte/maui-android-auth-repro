using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Identity.Client;
using Sfinx.ClientApp.Services.DeepLink;

namespace Sfinx.ClientApp
{
    [IntentFilter(new[] {Intent.ActionView},
            Categories = new[]
            {
                //Intent.ActionView,
                Intent.CategoryDefault,
                Intent.CategoryBrowsable
            }, AutoVerify = true,
            DataScheme = "sfinxinside", DataHost = "app"
        )
    ]
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density
        // Prevents MainActivitiy from being re-created on launching an intent (also makes it to where `OnNewIntent` will be called directly, if the app has already been loaded)
        , LaunchMode = LaunchMode.SingleTop
        )]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // In case the app was opened (on first load) with an `ActionView` intent
            OnNewIntent(this.Intent);
            // CheckForAppLink(Intent);
        }
        
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            var data = intent.DataString;

            if (intent.Action != Intent.ActionView) return;
            if (string.IsNullOrWhiteSpace(data)) return;

            var appServices = ServiceHelper.Current.GetRequiredService<DeeplinkAppService>(); // MAUI cross-platform service resolver: https://stackoverflow.com/a/73521158/10388359
            var relativeUri = intent.Data.Path + "?" + intent.Data.Query;
            var link = new Uri(relativeUri);
            appServices.OnAppLinkReceived(relativeUri);
        }

        /// <summary>
        /// A method to check if an application has been opened using a Universal link.
        /// Android implementation.
        /// </summary>
        /// <param name="intent"></param>
        private void CheckForAppLink(Intent intent)
        {
            var action = intent.Action;
            var strLink = intent.DataString;
            if (Intent.ActionView != action || string.IsNullOrWhiteSpace(strLink))
                return;
            var relativeUri = intent.Data.Path + "?" + intent.Data.Query;
            var link = new Uri(relativeUri);
            //DeepLinkService.SetPage(relativeUri);
        }
    }
    

}