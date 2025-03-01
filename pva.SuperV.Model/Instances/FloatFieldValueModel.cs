using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record FloatFieldValueModel(float Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
