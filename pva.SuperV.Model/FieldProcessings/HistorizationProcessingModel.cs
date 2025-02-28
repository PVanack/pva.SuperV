namespace pva.SuperV.Model.FieldProcessings
{
    public record HistorizationProcessingModel(string Name,
            string TrigerringFieldName,
            string HistoryRepositoryName,
            string? TimestampFieldName,
            List<string> FieldsToHistorize)
            : FieldValueProcessingModel(Name, TrigerringFieldName)
    {
    }
}
