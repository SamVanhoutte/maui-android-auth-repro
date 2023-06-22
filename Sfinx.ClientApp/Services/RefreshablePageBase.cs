using Microsoft.AspNetCore.Components;
using Sfinx.ClientApp.Services.DeepLink;

public abstract class RefreshablePageBase : ComponentBase, IDisposable
{
    public static RefreshablePageBase Current;
    
    [Parameter]
    [SupplyParameterFromQuery(Name = "forcerefresh")]
    public string? RefreshParameter { get; set; }
    
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public DeeplinkAppService DeeplinkService { get; set; }
    protected RefreshablePageBase()
    {
        Current = this;
    }

    protected override Task OnInitializedAsync()
    {
        DeeplinkService.AppLinkReceived += AppServices_AppLinkReceived;

        if (!string.IsNullOrEmpty(DeeplinkService.LastAppLink))
        {
            AppServices_AppLinkReceived(null, new() { Data = DeeplinkService.LastAppLink });
        }
        return base.OnInitializedAsync();
    }

    private void AppServices_AppLinkReceived(object? sender, AppLinkReceivedEventArgs e)
    {
        NavigationManager.NavigateTo(e.Data, forceLoad:true);
    }
    
    void IDisposable.Dispose()
    {
        DeeplinkService.AppLinkReceived -= AppServices_AppLinkReceived;
    }
    
    public bool ShouldRefresh
    {
        get
        {
            if (bool.TryParse(RefreshParameter, out var shouldRefresh))
            {
                return shouldRefresh;
            }

            return false;
        }
    }
    
    // protected void CheckRequestedPage()
    // {
    //     if (!string.IsNullOrWhiteSpace(deeplinkAppService.Page))
    //     {
    //         NavigationManager.NavigateTo(DeepLinkService.Page, forceLoad:true);
    //         DeepLinkService.SetPage("");
    //     }
    // }
}