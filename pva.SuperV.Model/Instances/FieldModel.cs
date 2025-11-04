using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Field of an instance.")]
    [ExcludeFromCodeCoverage]
    public record FieldModel(
        [property:Description("Field name")]
        string Name, string Type,
        [property:Description("Field value")]
        FieldValueModel FieldValue);
}
