using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("History retrieval result.")]
    public record HistoryStatisticsResultModel(
        [property:Description("Header containing fields information.")]
        List<HistoryStatisticResultFieldModel> Header,
        [property:Description("Rows of history values.")]
        List<HistoryStatisticsRowModel> Rows)
    {
    }
}
