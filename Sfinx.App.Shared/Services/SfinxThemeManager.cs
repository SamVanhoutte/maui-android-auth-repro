using MudBlazor;
using MudBlazor.Utilities;
using Sfinx.App.Shared.Interfaces;
using Sfinx.Backend.WebAPI.Client;

namespace Sfinx.App.Shared.Services;

public class SfinxThemeManager : IThemeManager
{
    public MudTheme DefaultTheme => new MudTheme
    {
        Palette = new Palette()
        {
            Primary = new MudColor("#132040"), Secondary = new MudColor("#2DD497"), Tertiary = new MudColor("#7A87A7"),
            Info = new MudColor("#8995B2")
        },
        Typography = new Typography()
        {
            H1 = new H1()
            {
                FontSize = "32px", FontFamily = new[] {"Avenir", "sans-serif"}, LineHeight = 1, FontWeight = 900,
                LetterSpacing = "1px",
            },
            H2 = new H2()
            {
                FontSize = "24px", FontFamily = new[] {"Avenir", "sans-serif"}, LineHeight = 1, FontWeight = 600,
                LetterSpacing = "1px",
            },
            H3 = new H3() {FontSize = "28px", FontFamily = new[] {"Avenir", "sans-serif"}},
            Body1 = new Body1()
                {FontSize = "16px", FontFamily = new[] {"Avenir", "sans-serif"}, LineHeight = 2, FontWeight = 400},
            Subtitle1 = new Subtitle1()
            {
                FontSize = "20px", FontFamily = new[] {"Avenir", "sans-serif"}, LineHeight = 2, FontWeight = 800,
                LetterSpacing = "1px"
            },
            Subtitle2 = new Subtitle2()
            {
                FontSize = "0.5rem", FontFamily = new[] {"Avenir", "sans-serif"}, LineHeight = 2, FontWeight = 800,
                LetterSpacing = "1px"
            },
            Caption = new Caption()
            {
                FontSize = "10px", FontFamily = new[] {"Avenir", "sans-serif"}, TextTransform = "uppercase",
                LineHeight = 1, FontWeight = 800, LetterSpacing = "1px",
            }
        }
    };

    public string GetLockIcon(LockStatus? status, VendorAuthenticationState? authState, bool errorState)
    {
        if (errorState)
            return Icons.Material.Filled.Error;
        if (authState != null)
        {
            if (authState?.TokenState != TokenState.Valid)
            {
                return Icons.Material.Filled.Error;
            }
        }

        switch (status)
        {
            case LockStatus.Unlocked:
                return Icons.Material.Filled.LockOpen;
            case LockStatus.Locked:
                return Icons.Material.Filled.Lock;
            case LockStatus.Unknown:
            default:
                return Icons.Material.Outlined.Lock;
        }
    }

    public Color GetLockColor(LockStatus? status, VendorAuthenticationState? authState, bool errorState)
    {
        if (authState != null)
        {
            if (authState.TokenState != TokenState.Valid)
            {
                return Color.Error;
            }
        }

        if (errorState)
        {
            return Color.Error;
        }

        switch (status)
        {
            case LockStatus.Unlocked:
                return Color.Warning;
            case LockStatus.Locked:
                return Color.Success;
            case LockStatus.Unknown:
            default:
                return Color.Primary;
        }
    }

    public Color GetBatteryColor(BatteryLevel? status, VendorAuthenticationState? authState)
    {
        switch (status)
        {
            case BatteryLevel.Healthy:
            case BatteryLevel.High:
            case BatteryLevel.Full:
                return Color.Success;
            case BatteryLevel.Low:
                return Color.Warning;
            case BatteryLevel.Critical:
                return Color.Error;
            case null:
            default:
                return Color.Primary;
        }
    }
}