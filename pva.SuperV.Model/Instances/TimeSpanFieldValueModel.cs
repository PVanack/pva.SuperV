using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.Instances
{
    [Description("Time span field value")]
    public record TimeSpanFieldValueModel(
        [property: Description("Field value.")]
        TimeSpan? Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp)
    {
    }
}
