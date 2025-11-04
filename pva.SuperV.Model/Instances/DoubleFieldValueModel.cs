using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Double field value")]
    [ExcludeFromCodeCoverage]
    public record DoubleFieldValueModel(
        [property: Description("Field value.")]
        double Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp);
}
