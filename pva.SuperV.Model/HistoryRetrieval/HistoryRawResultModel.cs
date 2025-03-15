using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("History retrieval result.")]
    public record HistoryRawResultModel(
        [property:Description("Header containing fields information.")]
        List<HistoryFieldModel> Header,
        [property:Description("Rows of history values.")]
        List<HistoryRawRowModel> Rows)
    {
    }
}
