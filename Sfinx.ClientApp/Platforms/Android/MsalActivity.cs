using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace Sfinx.ClientApp;

[Activity(Exported = true)]
[IntentFilter(new[] {Intent.ActionView},
    Categories = new[] {Intent.CategoryBrowsable, Intent.CategoryDefault},
    DataHost = "auth",
    DataScheme = "msal51547e7c-9522-40a9-b840-d3e9b7eeeb4e")]
public class MsalDevActivity : BrowserTabActivity
{
}

[Activity(Exported = true)]
[IntentFilter(new[] {Intent.ActionView},
    Categories = new[] {Intent.CategoryBrowsable, Intent.CategoryDefault},
    DataHost = "auth",
    DataScheme = "msalacc0762f-2d44-48a9-a2c4-7df3b34ad2ea")]
public class MsalPrdActivity : BrowserTabActivity
{
}

[Activity(Exported = true)]
[IntentFilter(new[] {Intent.ActionView},
    Categories = new[] {Intent.CategoryBrowsable, Intent.CategoryDefault},
    DataHost = "auth",
    DataScheme = "msalc480acb6-2ddf-4a33-8bca-f572e223a0d8")]
public class MsalUatActivity : BrowserTabActivity
{
}