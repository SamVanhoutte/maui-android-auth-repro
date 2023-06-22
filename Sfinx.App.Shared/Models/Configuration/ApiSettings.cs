namespace Sfinx.App.Shared.Models.Configuration;

public class ApiSettings
{
    public Dictionary<string, ApiEnvironmentSetting> EnvironmentSettings { get; set; }
}


public class ApiEnvironmentSetting{
    public string BaseUri { get; set; } = "";
    public string PublicUri { get; set; }= "";
    public string SubscriptionKey { get; set; }= "";
    
    public AzureB2CSettings AuthSettings { get; set; }
}