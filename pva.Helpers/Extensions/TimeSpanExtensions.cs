using System.Globalization;

namespace pva.Helpers.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan ParseTimeSpanInvariant(this string intervalString)
        {
            ArgumentNullException.ThrowIfNull(intervalString);
            return TimeSpan.Parse(intervalString, CultureInfo.InvariantCulture.DateTimeFormat);
        }

    }
}
