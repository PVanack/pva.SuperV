using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Date and time field value")]
    [ExcludeFromCodeCoverage]
    public record DateTimeFieldValueModel(
        [property: Description("Field value.")]
        DateTime Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp);
}
