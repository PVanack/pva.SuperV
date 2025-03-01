using pva.SuperV.Engine;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model.Instances
{
    [JsonDerivedType(typeof(BoolFieldValueModel), typeDiscriminator: nameof(BoolFieldValueModel))]
    [JsonDerivedType(typeof(DateTimeFieldValueModel), typeDiscriminator: nameof(DateTimeFieldValueModel))]
    [JsonDerivedType(typeof(FloatFieldValueModel), typeDiscriminator: nameof(FloatFieldValueModel))]
    [JsonDerivedType(typeof(IntFieldValueModel), typeDiscriminator: nameof(IntFieldValueModel))]
    [JsonDerivedType(typeof(LongFieldValueModel), typeDiscriminator: nameof(LongFieldValueModel))]
    [JsonDerivedType(typeof(ShortFieldValueModel), typeDiscriminator: nameof(ShortFieldValueModel))]
    [JsonDerivedType(typeof(StringFieldValueModel), typeDiscriminator: nameof(StringFieldValueModel))]
    [JsonDerivedType(typeof(TimeSpanFieldValueModel), typeDiscriminator: nameof(TimeSpanFieldValueModel))]
    [JsonDerivedType(typeof(UintFieldValueModel), typeDiscriminator: nameof(UintFieldValueModel))]
    [JsonDerivedType(typeof(UlongFieldValueModel), typeDiscriminator: nameof(UlongFieldValueModel))]
    [JsonDerivedType(typeof(UshortFieldValueModel), typeDiscriminator: nameof(UshortFieldValueModel))]
    public abstract record FieldValueModel(QualityLevel? Quality, DateTime? Timestamp)
    {
    }
}
