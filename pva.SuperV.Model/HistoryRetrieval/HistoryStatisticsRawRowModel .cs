using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("Row of field values from history")]
    public record HistoryStatisticsRawRowModel(
        [property: Description("Timestamp of row.")]
        DateTime Timestamp,
        [property: Description("Start timestamp of interval of row in case of interpolated values.")]
        DateTime? StartTime,
        [property: Description("End timestamp of interval of row in case of interpolated values.")]
        DateTime? EndTime,
        [property: Description("Duration of interval of row in case of interpolated values.")]
        TimeSpan? Duration,
        [property: Description("Quality level of row.")]
        QualityLevel Quality,
        [property: Description("Retrieved values.")]
        List<object> FieldValues)
    {
    }
}