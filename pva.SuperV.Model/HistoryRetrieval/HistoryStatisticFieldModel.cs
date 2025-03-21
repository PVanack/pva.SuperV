using pva.SuperV.Engine.HistoryRetrieval;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("History statistic field model")]
    [ExcludeFromCodeCoverage]
    public record HistoryStatisticFieldModel(
        [property:Description("Name of field.")]
        string Name,
        [property:Description("Statistic function to be applied to field.")]
        HistoryStatFunction StatisticFunction)
    {
    }
}
