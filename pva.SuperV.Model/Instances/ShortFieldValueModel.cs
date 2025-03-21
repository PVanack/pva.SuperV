using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Short field value")]
    [ExcludeFromCodeCoverage]
    public record ShortFieldValueModel(
        [property: Description("Field value.")]
        short Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp)
    {
    }
}
