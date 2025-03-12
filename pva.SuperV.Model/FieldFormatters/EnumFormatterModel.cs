using pva.SuperV.Engine.FieldValueFormatters;
using System.ComponentModel;

namespace pva.SuperV.Model.FieldFormatters
{
    [Description("Enum Field formatter")]
    public record EnumFormatterModel(
        string Name,
        [property: Description("String values associated with integers.")]
        Dictionary<int, string> Values)
        : FieldFormatterModel(Name, typeof(EnumFormatter).ToString())
    {
    }
}