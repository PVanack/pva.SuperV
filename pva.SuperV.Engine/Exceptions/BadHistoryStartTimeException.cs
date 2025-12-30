using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unhandled field type for history storage engine is referenced.</summary>
    public class BadHistoryStartTimeException : SuperVException
    {
        public BadHistoryStartTimeException(DateTime startTime, DateTime endTime)
            : base($"StartTime ({startTime}) shoud be less than EndTime ({endTime}).")
        {
        }

        [ExcludeFromCodeCoverage]
        public BadHistoryStartTimeException(string? message) : base(message)
        {

        }

        [ExcludeFromCodeCoverage]
        public BadHistoryStartTimeException()
        {
        }

        [ExcludeFromCodeCoverage]
        public BadHistoryStartTimeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}