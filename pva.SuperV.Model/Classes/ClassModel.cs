using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Classes
{
    [Description("Class")]
    [ExcludeFromCodeCoverage]
    public record ClassModel(
        [property: Description("Name of the class.")]
        string Name,
        [property: Description("Base class (if any).")]
        string? BaseClassName)
    {
    }
}
