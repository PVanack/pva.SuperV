using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.Instances
{
    [Description("Date and time field value")]
    public record DateTimeFieldValueModel(
        [property: Description("Field value.")]
        DateTime Value,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(Quality, Timestamp)
    {
    }
}
