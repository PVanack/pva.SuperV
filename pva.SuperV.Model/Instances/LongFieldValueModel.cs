using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.Instances
{
    [Description("Long field value")]
    public record LongFieldValueModel(
        [property: Description("Field value.")]
        long? Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp)
    {
    }
}
