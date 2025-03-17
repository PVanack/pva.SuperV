using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.Instances
{
    [Description("Unsigned int field value")]
    public record UintFieldValueModel(
        [property: Description("Field value.")]
        uint? Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp)
    {
    }
}
