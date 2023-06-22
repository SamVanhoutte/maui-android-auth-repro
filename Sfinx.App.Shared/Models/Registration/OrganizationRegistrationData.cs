using Sfinx.App.Shared.Services.MultiLinguality;

namespace Sfinx.App.Shared.Models.Registration;

public class OrganizationRegistrationData
{
    public static OrganizationRegistrationData Load(string organizationName, TranslatorService localizer)
    {
        if (string.IsNullOrEmpty(organizationName)) organizationName = "sfinx";
        organizationName = organizationName.ToLower();
        return new OrganizationRegistrationData
        {
            OrganizationName = localizer[$"{organizationName}/OrganizationName"],
            ProjectName = localizer[$"{organizationName}/ProjectName"],
            SfinxRegistrationText = localizer[$"{organizationName}/SfinxRegistrationText"],
            WelcomeText = localizer[$"{organizationName}/WelcomeText"],
            CompletionText = localizer[$"{organizationName}/CompletionText"],
        };
    }

    public string OrganizationName { get; set; }= "";
    public string ProjectName { get; set; }= "";
    public string WelcomeText { get; set; }= "";
    public string SfinxRegistrationText { get; set; }= "";
    public string CompletionText { get; set; }
}