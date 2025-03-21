using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("Row of field values from history")]
    [ExcludeFromCodeCoverage]
    public record HistoryStatisticsRowModel(
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
        List<FieldValueModel> FieldValues)
    {
    }
}