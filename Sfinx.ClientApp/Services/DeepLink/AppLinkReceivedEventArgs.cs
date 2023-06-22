namespace Sfinx.ClientApp.Services.DeepLink;

public class AppLinkReceivedEventArgs : EventArgs
{
    public required string Data { get; set; }
}