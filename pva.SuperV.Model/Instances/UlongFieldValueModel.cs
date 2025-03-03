using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.Instances
{
    [Description("Unsigned long field value")]
    public record UlongFieldValueModel(
        [property: Description("Field value.")]
        ulong Value,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(Quality, Timestamp)
    {
    }
}
