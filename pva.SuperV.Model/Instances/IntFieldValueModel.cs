using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record IntFieldValueModel(int Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
