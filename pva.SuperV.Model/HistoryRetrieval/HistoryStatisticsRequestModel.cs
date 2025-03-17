using pva.SuperV.Engine.HistoryRetrieval;
using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("Request on fields' history")]
    public record HistoryStatisticsRequestModel(
    [property:Description("Start time of request.")]
    DateTime StartTime,
    [property:Description("End time of request.")]
    DateTime EndTime,
    [property:Description("Time span of interpolation. If null, indicates to use actual history values.")]
    TimeSpan InterpolationInterval,
    [property:Description("Fill mode in case the interval misses values.")]
    FillMode? FillMode,
    [property:Description("List of fields to be retrieved.")]
    List<HistoryStatisticFieldModel> HistoryFields)
    {
    }
}
