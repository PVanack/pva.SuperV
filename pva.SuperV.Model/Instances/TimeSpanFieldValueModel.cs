using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Time span field value")]
    [ExcludeFromCodeCoverage]
    public record TimeSpanFieldValueModel(
        [property: Description("Field value.")]
        TimeSpan Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp);
}
