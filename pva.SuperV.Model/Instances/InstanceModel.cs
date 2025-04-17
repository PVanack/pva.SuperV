using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Instances
{
    [Description("Instance of a class")]
    [ExcludeFromCodeCoverage]
    public record InstanceModel(
        [property: Description("Instance name.")]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(Engine.Constants.IdentifierNamePattern, ErrorMessage = "Must be a valid identifier")]
        string Name,
        [property: Description("Class name of instance.")]
        string ClassName,
        [property: Description("Instance field values.")]
        List<FieldModel> Fields)
    {
    }
}
