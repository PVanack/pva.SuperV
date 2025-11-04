using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Boolean field value")]
    [ExcludeFromCodeCoverage]
    public record BoolFieldValueModel(
        [property: Description("Field value.")]
        bool Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp);
}
