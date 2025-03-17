using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("History statistics retrieval result.")]
    public record HistoryStatisticsRawResultModel(
        [property:Description("Header containing fields information.")]
        List<HistoryStatisticResultFieldModel> Header,
        [property:Description("Rows of history values.")]
        List<HistoryStatisticsRawRowModel> Rows)
    {
    }
}
