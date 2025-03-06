using System.ComponentModel;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Field definition")]
    [JsonDerivedType(typeof(BoolFieldDefinitionModel), typeDiscriminator: nameof(BoolFieldDefinitionModel))]
    [JsonDerivedType(typeof(DateTimeFieldDefinitionModel), typeDiscriminator: nameof(DateTimeFieldDefinitionModel))]
    [JsonDerivedType(typeof(DoubleFieldDefinitionModel), typeDiscriminator: nameof(DoubleFieldDefinitionModel))]
    [JsonDerivedType(typeof(FloatFieldDefinitionModel), typeDiscriminator: nameof(FloatFieldDefinitionModel))]
    [JsonDerivedType(typeof(IntFieldDefinitionModel), typeDiscriminator: nameof(IntFieldDefinitionModel))]
    [JsonDerivedType(typeof(LongFieldDefinitionModel), typeDiscriminator: nameof(LongFieldDefinitionModel))]
    [JsonDerivedType(typeof(ShortFieldDefinitionModel), typeDiscriminator: nameof(ShortFieldDefinitionModel))]
    [JsonDerivedType(typeof(StringFieldDefinitionModel), typeDiscriminator: nameof(StringFieldDefinitionModel))]
    [JsonDerivedType(typeof(TimeSpanFieldDefinitionModel), typeDiscriminator: nameof(TimeSpanFieldDefinitionModel))]
    [JsonDerivedType(typeof(UintFieldDefinitionModel), typeDiscriminator: nameof(UintFieldDefinitionModel))]
    [JsonDerivedType(typeof(UlongFieldDefinitionModel), typeDiscriminator: nameof(UlongFieldDefinitionModel))]
    [JsonDerivedType(typeof(UshortFieldDefinitionModel), typeDiscriminator: nameof(UshortFieldDefinitionModel))]
    public abstract record FieldDefinitionModel(
        [property: Description("Name of field")] string Name,
        [property: Description("Type of field")] string Type,
        [property: Description("Field value formatter")] string? ValueFormatter)
    {
    }
}
