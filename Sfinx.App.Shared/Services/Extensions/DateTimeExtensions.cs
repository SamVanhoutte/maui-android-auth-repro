using System.Globalization;
using Sfinx.App.Shared.Services.MultiLinguality;

namespace Sfinx.App.Shared.Services.Extensions;

public static class DateTimeExtensions
{
    public static string PrintHistoricalTime(this DateTime utcTimestamp, TranslatorService translatorService)
    {
        var currentDate = DateTime.UtcNow;
        if (currentDate <= utcTimestamp)
        {
            return translatorService["time/now"];
        }

        var timeAgo = currentDate - utcTimestamp;
        // If less than a minute
        if (timeAgo < TimeSpan.FromMinutes(1))
        {
            return string.Format(translatorService["time/secondsago"], timeAgo.Seconds);
        }

        // If less than an hour
        if (timeAgo < TimeSpan.FromHours(1))
        {
            return string.Format(translatorService["time/minutesago"], timeAgo.Minutes);
        }

        // If today
        if (timeAgo < TimeSpan.FromHours(24) && currentDate.Day == utcTimestamp.Day)
        {
            return $"{translatorService["time/today"]} {utcTimestamp:HH:mm}";
        }

        // If this week
        if (timeAgo < TimeSpan.FromDays(7))
        {
            return
                $"{CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(utcTimestamp.DayOfWeek)} {utcTimestamp:HH:mm}";
        }

        // Just time print
        return utcTimestamp.ToString("g");
    }
    
    public static string Print(this DateTimeOffset? utcTimestamp)
    {
        if (utcTimestamp == null) return "-";
        return utcTimestamp.Value.ToString("f");
    }
}