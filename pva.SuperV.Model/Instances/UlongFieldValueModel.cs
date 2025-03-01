using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record UlongFieldValueModel(ulong Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
