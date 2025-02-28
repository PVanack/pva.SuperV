using System.Text.Json.Serialization;

namespace pva.SuperV.Model.FieldProcessings
{
    [JsonDerivedType(typeof(AlarmStateProcessingModel), typeDiscriminator: nameof(AlarmStateProcessingModel))]
    [JsonDerivedType(typeof(HistorizationProcessingModel), typeDiscriminator: nameof(HistorizationProcessingModel))]
    public abstract record FieldValueProcessingModel(
        string Name,
        string TrigerringFieldType,
        string TrigerringFieldName)
    {
    }
}
