using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record LongFieldValueModel(long Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
