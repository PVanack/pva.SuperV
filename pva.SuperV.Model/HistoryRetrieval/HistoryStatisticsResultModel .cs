using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("History retrieval result.")]
    [ExcludeFromCodeCoverage]
    public record HistoryStatisticsResultModel(
        [property:Description("Header containing fields information.")]
        List<HistoryStatisticResultFieldModel> Header,
        [property:Description("Rows of history values.")]
        List<HistoryStatisticsRowModel> Rows)
    {
    }
}
