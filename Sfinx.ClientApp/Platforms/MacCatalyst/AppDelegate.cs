using Foundation;
using UIKit;

namespace Sfinx.ClientApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem,
        UIOperationHandler completionHandler)
    {
        base.PerformActionForShortcutItem(application, shortcutItem, completionHandler);
    }
}