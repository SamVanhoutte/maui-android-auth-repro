using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Sfinx.App.Shared.Interfaces;

namespace Sfinx.ClientApp.Services;

public class AppAlertNotifier : IAlertNotifier
{
    public async Task ShowAlertAsync(string message, string title = null, bool isError = false)
    {
        var snackbar = Snackbar.Make(message, actionButtonText: "OK", duration: TimeSpan.FromSeconds(5),
            visualOptions: new SnackbarOptions
            {
                CornerRadius = new CornerRadius(10), BackgroundColor = Colors.Orange, TextColor = Colors.Black,
                CharacterSpacing = 0.5
            });
        await snackbar.Show();
    }
}