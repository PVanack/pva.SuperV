using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record StringFieldValueModel(string Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
