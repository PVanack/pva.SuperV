using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Int field value")]
    [ExcludeFromCodeCoverage]
    public record IntFieldValueModel(
        [property: Description("Field value.")]
        int Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp)
    {
    }
}
