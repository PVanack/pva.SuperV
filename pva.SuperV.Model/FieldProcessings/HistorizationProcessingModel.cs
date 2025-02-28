namespace pva.SuperV.Model.FieldProcessings
{
    public record HistorizationProcessingModel(string Name,
            string TrigerringFieldType,
            string TrigerringFieldName,
            string HistoryRepositoryName,
            string? TimestampFieldName,
            List<string> FieldsToHistorize)
            : FieldValueProcessingModel(Name, TrigerringFieldType, TrigerringFieldName)
    {
    }
}
