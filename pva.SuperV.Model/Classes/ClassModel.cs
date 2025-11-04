using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Classes
{
    [Description("Class")]
    [ExcludeFromCodeCoverage]
    public record ClassModel(
        [property: Description("Name of the class.")]
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(Engine.Constants.IdentifierNamePattern, ErrorMessage = "Must be a valid identifier")]
        string Name,
        [property: Description("Base class (if any).")]
        string? BaseClassName);
}
