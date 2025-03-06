using pva.SuperV.Engine;
using System.ComponentModel;

namespace pva.SuperV.Model.Instances
{
    [Description("String field value")]
    public record StringFieldValueModel(
        [property: Description("Field value.")]
        string Value,
        QualityLevel? Quality,
        DateTime? Timestamp)
        : FieldValueModel(null, Quality, Timestamp)
    {
    }
}
