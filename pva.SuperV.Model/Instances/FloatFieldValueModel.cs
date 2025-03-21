using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Float field value")]
    [ExcludeFromCodeCoverage]
    public record FloatFieldValueModel(
        [property: Description("Field value.")]
        float Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp)
    {
    }
}
