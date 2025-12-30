using TDengine.Driver;

namespace pva.SuperV.Engine.HistoryRetrieval
{
    /// <summary>
    /// Row of history for a specific timestamp.
    /// </summary>
    /// <remarks>
    /// Builds a row from a TDengine row.
    /// </remarks>
    /// <param name="row">TDengine row</param>
    /// <param name="fields">List of statistic fields.</param>
    /// /// <param name="keepFieldType">Whether to convert values to field type or keep as is.</param>
    public class HistoryStatisticRow(IRows row, List<HistoryStatisticField> fields, bool keepFieldType)
        : HistoryRow(row, [.. fields.Select(field => field.Field)], keepFieldType)
    {
        /// <summary>
        /// Start time of interval if an interval was specified.
        /// </summary>
        public DateTime StartTime { get; } = ((DateTime)row.GetValue(row.FieldCount - 5)).ToUniversalTime();

        /// <summary>
        /// End time of interval if an interval was specified.
        /// </summary>
        public DateTime EndTime { get; } = ((DateTime)row.GetValue(row.FieldCount - 4)).ToUniversalTime();

        /// <summary>
        /// Duration of interval if an interval was specified.
        /// </summary>
        public TimeSpan Duration { get; } = TimeSpan.FromMicroseconds(((long)row.GetValue(row.FieldCount - 3)) / 1000);
    }
}
