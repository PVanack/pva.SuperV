using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.Instances
{
    [Description("Double field value")]
    public record DoubleFieldValueModel(
        [property: Description("Field value.")]
        double Value,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(Quality, Timestamp)
    {
    }
}
