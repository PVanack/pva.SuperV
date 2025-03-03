using System.ComponentModel;

namespace pva.SuperV.Model.FieldProcessings
{
    public record HistorizationProcessingModel(
        string Name,
        string TrigerringFieldName,
        [property: Description("History repository name.")]
        string HistoryRepositoryName,
        [property: Description("Timestamp field name.")]
        string? TimestampFieldName,
        [property: Description("List of field whos value is to be historized.")]
        List<string> FieldsToHistorize)
        : FieldValueProcessingModel(Name, TrigerringFieldName)
    {
    }
}
