using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model.FieldProcessings
{
    [Description("Field value processing")]
    [ExcludeFromCodeCoverage]
    [JsonDerivedType(typeof(AlarmStateProcessingModel), typeDiscriminator: nameof(AlarmStateProcessingModel))]
    [JsonDerivedType(typeof(HistorizationProcessingModel), typeDiscriminator: nameof(HistorizationProcessingModel))]
    public abstract record FieldValueProcessingModel(
        [property: Description("Name of field value change processing.")]
        [Required(AllowEmptyStrings = false)]
        string Name,
        [property: Description("Name of field triggering the processing.")]
        [Required(AllowEmptyStrings = false)]
        string TrigerringFieldName)
    {
    }
}
