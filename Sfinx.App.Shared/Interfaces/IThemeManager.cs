using MudBlazor;
using MudBlazor.Utilities;
using Sfinx.Backend.WebAPI.Client;

namespace Sfinx.App.Shared.Interfaces;

public interface IThemeManager
{
     MudTheme DefaultTheme { get; }
     string GetLockIcon(LockStatus? status, VendorAuthenticationState? authState, bool errorState);
     Color GetLockColor(LockStatus? status, VendorAuthenticationState? authState,bool errorState);
     Color GetBatteryColor(BatteryLevel? status,VendorAuthenticationState? authState);
}