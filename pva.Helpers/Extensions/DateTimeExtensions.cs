using System.Globalization;

namespace pva.Helpers.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ParseDateTimeInvariant(this string fromString)
        {
            return DateTime.Parse(fromString, CultureInfo.InvariantCulture.DateTimeFormat).ToUniversalTime();
        }
    }
}
