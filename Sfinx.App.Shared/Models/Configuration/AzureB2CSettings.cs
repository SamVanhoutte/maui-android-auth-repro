using Microsoft.Extensions.Options;

namespace Sfinx.App.Shared.Models.Configuration;



public class AzureB2CSettings
{
    public string ClientId { get; set; }
    public string TenantName  { get; set; }
    public string SignInPolicy{ get; set; }
    public string TenantId => $"{TenantName}.onmicrosoft.com";
    public string[] Scopes => new string[] {"openid", "offline_access", $"https://{TenantId}/backend-api/Lock.OpenClose"};
    public string AuthorityBase => $"https://{TenantName}.b2clogin.com/tfp/{TenantId}/";
    public string AuthoritySignIn => $"{AuthorityBase}{SignInPolicy}";
}


// public class AzureB2CSettings
// {
//     public static readonly string ClientId = "51547e7c-9522-40a9-b840-d3e9b7eeeb4e";
//     public static readonly string TenantName = "sfinxdevb2c";
//     public static readonly string SignInPolicy = "B2C_1_signup_sfinx_user";
//     public static readonly string TenantId = $"{TenantName}.onmicrosoft.com";
//     public static readonly string[] Scopes = new string[] { "openid", "offline_access", $"https://{TenantId}/backend-api/Lock.OpenClose" };
//     public static readonly string AuthorityBase = $"https://{TenantName}.b2clogin.com/tfp/{TenantId}/";
//     public static readonly string AuthoritySignIn = $"{AuthorityBase}{SignInPolicy}";
// }