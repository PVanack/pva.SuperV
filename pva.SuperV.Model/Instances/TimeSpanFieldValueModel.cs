using pva.SuperV.Engine;

namespace pva.SuperV.Model.Instances
{
    public record TimeSpanFieldValueModel(TimeSpan Value, QualityLevel? Quality, DateTime? Timestamp) : FieldValueModel(Quality, Timestamp)
    {
    }
}
