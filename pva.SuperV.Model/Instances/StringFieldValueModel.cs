using pva.SuperV.Engine;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("String field value")]
    [ExcludeFromCodeCoverage]
    public record StringFieldValueModel(
        [property: Description("Field value.")]
        string? Value,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(null, Quality, Timestamp)
    {
    }
}
