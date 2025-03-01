using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record UshortFieldValueModel(ushort Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
