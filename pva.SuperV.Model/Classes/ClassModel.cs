using System.ComponentModel;

namespace pva.SuperV.Model.Classes
{
    [Description("Class")]
    public record ClassModel(
        [property: Description("Name of the class.")]
        string Name,
        [property: Description("Base class (if any).")]
        string? BaseClassName)
    {
    }
}
