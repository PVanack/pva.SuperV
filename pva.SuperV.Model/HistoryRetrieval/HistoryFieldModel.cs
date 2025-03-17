using System.ComponentModel;

namespace pva.SuperV.Model.HistoryRetrieval
{
    [Description("History field model")]
    public record HistoryFieldModel(
        [property:Description("Name of field")]
        string Name,
        [property:Description("Type of field")]
        string Type,
        [property:Description("Column index of field in rows")]
        int ColumnIndex)
    {
    }
}