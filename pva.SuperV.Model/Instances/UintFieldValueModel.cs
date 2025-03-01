using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record UintFieldValueModel(uint Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
