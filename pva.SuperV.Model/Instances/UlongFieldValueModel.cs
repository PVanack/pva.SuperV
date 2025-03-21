using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Unsigned long field value")]
    [ExcludeFromCodeCoverage]
    public record UlongFieldValueModel(
        [property: Description("Field value.")]
        ulong Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp)
    {
    }
}
