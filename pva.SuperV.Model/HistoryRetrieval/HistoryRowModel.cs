using pva.SuperV.Engine;
using pva.SuperV.Model.Instances;
using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("Row of field values from history")]
    public record HistoryRowModel(
        [property: Description("Timestamp of row.")]
        DateTime Timestamp,
        [property: Description("Quality level of row.")]
        QualityLevel Quality,
        [property: Description("Retrieved values.")]
        List<FieldValueModel> FieldValues)
    {
    }
}