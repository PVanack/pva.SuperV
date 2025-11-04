using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldProcessings
{
    [Description("Historization field processing")]
    [ExcludeFromCodeCoverage]
    public record HistorizationProcessingModel(
        string Name,
        string TrigerringFieldName,
        [property: Description("History repository name.")]
        string HistoryRepositoryName,
        [property: Description("Timestamp field name.")]
        string? TimestampFieldName,
        [property: Description("List of field whos value is to be historized.")]
        List<string> FieldsToHistorize)
        : FieldValueProcessingModel(Name, TrigerringFieldName);
}
