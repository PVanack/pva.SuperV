using System.ComponentModel;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Field definition")]
    [JsonDerivedType(typeof(BoolFieldDefinitionModel), typeDiscriminator: nameof(BoolFieldDefinitionModel))]
    [JsonDerivedType(typeof(ShortFieldDefinitionModel), typeDiscriminator: nameof(ShortFieldDefinitionModel))]
    [JsonDerivedType(typeof(UshortFieldDefinitionModel), typeDiscriminator: nameof(UshortFieldDefinitionModel))]
    [JsonDerivedType(typeof(IntFieldDefinitionModel), typeDiscriminator: nameof(IntFieldDefinitionModel))]
    [JsonDerivedType(typeof(UintFieldDefinitionModel), typeDiscriminator: nameof(UintFieldDefinitionModel))]
    [JsonDerivedType(typeof(LongFieldDefinitionModel), typeDiscriminator: nameof(LongFieldDefinitionModel))]
    [JsonDerivedType(typeof(UlongFieldDefinitionModel), typeDiscriminator: nameof(UlongFieldDefinitionModel))]
    [JsonDerivedType(typeof(FloatFieldDefinitionModel), typeDiscriminator: nameof(FloatFieldDefinitionModel))]
    [JsonDerivedType(typeof(DoubleFieldDefinitionModel), typeDiscriminator: nameof(DoubleFieldDefinitionModel))]
    [JsonDerivedType(typeof(StringFieldDefinitionModel), typeDiscriminator: nameof(StringFieldDefinitionModel))]
    [JsonDerivedType(typeof(DateTimeFieldDefinitionModel), typeDiscriminator: nameof(DateTimeFieldDefinitionModel))]
    [JsonDerivedType(typeof(TimeSpanFieldDefinitionModel), typeDiscriminator: nameof(TimeSpanFieldDefinitionModel))]
    public abstract record FieldDefinitionModel(
        [property: Description("Name of field")] string Name,
        [property: Description("Type of field")] string Type)
    {
    }
}
