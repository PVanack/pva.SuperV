using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Instance of a class")]
    [ExcludeFromCodeCoverage]
    public record InstanceModel(
        [property: Description("Instance name.")]
        string Name,
        [property: Description("Class name of instance.")]
        string ClassName,
        [property: Description("Instance field values.")]
        List<FieldModel> Fields)
    {
    }
}
