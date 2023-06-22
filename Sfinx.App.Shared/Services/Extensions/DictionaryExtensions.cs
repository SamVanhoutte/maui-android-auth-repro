namespace Sfinx.App.Shared.Services.Extensions;

public static class DictionaryExtensions
{
    public static T? GetValue<T>(this IDictionary<string, T>? settings, string setting, T? defaultValue = default)
    {
        if (settings?.ContainsKey(setting)??false)
        {
            return settings[setting];
        }

        return defaultValue;
    }
}