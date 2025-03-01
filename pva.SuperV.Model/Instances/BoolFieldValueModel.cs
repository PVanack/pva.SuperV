using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record BoolFieldValueModel(bool Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
