using pva.SuperV.Engine.FieldFormatters;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldFormatters
{
    [Description("Enum Field formatter")]
    [ExcludeFromCodeCoverage]
    public record EnumFormatterModel(
        string Name,
        [property: Description("String values associated with integers.")]
        Dictionary<int, string> Values)
        : FieldFormatterModel(Name, typeof(EnumFormatter).ToString())
    {
    }
}