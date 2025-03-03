using System.ComponentModel;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model.FieldProcessings
{
    [JsonDerivedType(typeof(AlarmStateProcessingModel), typeDiscriminator: nameof(AlarmStateProcessingModel))]
    [JsonDerivedType(typeof(HistorizationProcessingModel), typeDiscriminator: nameof(HistorizationProcessingModel))]
    public abstract record FieldValueProcessingModel(
        [property: Description("Name of field value change processing.")]
        string Name,
        [property: Description("Name of field triggering the processing.")]
        string TrigerringFieldName)
    {
    }
}
