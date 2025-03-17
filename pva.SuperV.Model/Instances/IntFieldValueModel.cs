using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.Instances
{
    [Description("Int field value")]
    public record IntFieldValueModel(
        [property: Description("Field value.")]
        int? Value,
        string? FormattedValue,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(FormattedValue, Quality, Timestamp)
    {
    }
}
