using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record DoubleFieldValueModel(double Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
