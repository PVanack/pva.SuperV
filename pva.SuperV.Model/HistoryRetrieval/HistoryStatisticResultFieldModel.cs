using pva.SuperV.Engine.HistoryRetrieval;
using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("History statistic field model")]
    public record HistoryStatisticResultFieldModel(
        string Name,
        string Type,
        int ColumnIndex,
        [property:Description("Statistic function to be applied to field.")]
        HistoryStatFunction StatisticFunction)
        : HistoryFieldModel(Name, Type, ColumnIndex)
    {
    }
}
