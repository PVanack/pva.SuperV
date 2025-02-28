namespace pva.SuperV.Model.FieldProcessings
{
    public record AlarmStateProcessingModel(string Name,
            string TrigerringFieldType,
            string TrigerringFieldName,
            string? HighHighLimitFieldName,
            string HighLimitFieldName,
            string LowLimitFieldName,
            string? LowLowLimitFieldName,
            string? DeadbandFieldName,
            string AlarmStateFieldName,
            string? AckStateFieldName)
            : FieldValueProcessingModel(Name, TrigerringFieldType, TrigerringFieldName)
    {
    }
}
