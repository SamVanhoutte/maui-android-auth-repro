namespace Sfinx.App.Shared.Interfaces;

public interface IAlertNotifier
{
    Task ShowAlertAsync(string message, string? title = null, bool isError = false);
}