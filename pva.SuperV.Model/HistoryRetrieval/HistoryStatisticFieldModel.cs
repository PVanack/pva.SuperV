using pva.SuperV.Engine.HistoryRetrieval;
using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("History statistic field model")]
    public record HistoryStatisticFieldModel(
        [property:Description("Name of field.")]
        string Name,
        [property:Description("Statistic function to be applied to field.")]
        HistoryStatFunction StatisticFunction)
    {
    }
}
