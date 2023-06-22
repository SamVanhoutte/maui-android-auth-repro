using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Maui.LifecycleEvents;

namespace Sfinx.ClientApp;

using Flurl;
using Flurl.Util;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
    }

    void RefreshView_Refreshing(object sender, EventArgs e)
    {
        if (RefreshablePageBase.Current?.NavigationManager != null)
        {
            var navigationManager = RefreshablePageBase.Current.NavigationManager;
            var refreshUri = navigationManager.Uri.SetQueryParam("forcerefresh", "true");
            navigationManager.NavigateTo(refreshUri, true, true);
            RefreshView.IsRefreshing = false;
        }
    }

    private void WebView_OnUrlLoading(object sender, UrlLoadingEventArgs e)
    {
        if (e.Url.Scheme.Equals("sfinx", StringComparison.CurrentCultureIgnoreCase))
        {
            if (RefreshablePageBase.Current != null)
            {
                var navigationManager = RefreshablePageBase.Current.NavigationManager;
                navigationManager.NavigateTo(e.Url.ToString().Replace("sfinx://", ""));
            }
        }
    }

    private void MainPage_OnNavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
        Console.WriteLine("test");
    }

    private void MainPage_OnNavigatedTo(object sender, NavigatedToEventArgs e)
    {
        Console.WriteLine("test");
    }

    private void MainPage_OnNavigatingFrom(object sender, NavigatingFromEventArgs e)
    {
        Console.WriteLine("test");
    }

    private void WebView_OnBlazorWebViewInitializing(object sender, BlazorWebViewInitializingEventArgs e)
    {
        Console.WriteLine("test");
    }

    private void WebView_OnBlazorWebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
    {
        Console.WriteLine("test");
    }
}