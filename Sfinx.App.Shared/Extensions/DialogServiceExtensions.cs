using MudBlazor;
using Sfinx.App.Shared.Components.General;
using Sfinx.App.Shared.Services.MultiLinguality;

namespace Sfinx.App.Shared.Extensions;

public static class DialogServiceExtensions
{
    public static async Task<bool> AskConfirmationAsync(this IDialogService dialogService, TranslatorService translator,
        string title, string? message = null)
    {
        var parameters = new DialogParameters
        {
            {"ContentText", message ?? title},
            {"ButtonText", translator["Yes"]}
        };
        var dialogresult =
            await dialogService.ShowAsync<SfinxDialog>(title, parameters, SfinxDialog.StandardDialogOptions);
        var result = await dialogresult.Result;
        if (result.Canceled) return false;
        bool.TryParse(result.Data.ToString(), out bool confirmationResult);
        return confirmationResult;
    }
}