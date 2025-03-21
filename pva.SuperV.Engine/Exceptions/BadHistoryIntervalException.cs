using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unhandled field type for history storage engine is referenced.</summary>
    public class BadHistoryIntervalException : SuperVException
    {
        public BadHistoryIntervalException(TimeSpan interval, DateTime startTime, DateTime endTime)
            : base($"Interval ({interval}) should be less than EndTime - StartTime ({endTime - startTime}).")
        {
        }

        [ExcludeFromCodeCoverage]
        public BadHistoryIntervalException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public BadHistoryIntervalException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public BadHistoryIntervalException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}