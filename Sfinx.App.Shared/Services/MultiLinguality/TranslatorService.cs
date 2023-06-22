using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Sfinx.App.Shared.Services.MultiLinguality;



public class TranslatorService
{
    private readonly IStringLocalizer<SharedResources> sharedLocalizer;
    private readonly ILogger<TranslatorService> logger;

    public TranslatorService(
        IStringLocalizer<SharedResources> sharedLocalizer, ILogger<TranslatorService> logger)
    {
        this.sharedLocalizer = sharedLocalizer;
        this.logger = logger;
    }

    public string this[string lookupValue, [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""]
    {
        get
        {
            var localizedString = sharedLocalizer[lookupValue];
            if (localizedString.ResourceNotFound)
            {
                logger.LogWarning(
                    $"No translation found for {lookupValue} in the SharedResources with searched location {localizedString.SearchedLocation}");
            }

            return localizedString.Value;
        }
    }

    public string GetTitle([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        var page = GetPageName(sourceFilePath);
        Console.WriteLine($"Page : {page}");
        return this[$"{page}/Title"];
    }
    
    public string GetSubTitle([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        var page = GetPageName(sourceFilePath);
        Console.WriteLine($"Page : {page}");
        return this[$"{page}/Subtitle"];
    }

    private string GetPageName(string sourceFilePath)
    {
        return sourceFilePath.Split('/').Last().Replace(".razor", "");
    }
}