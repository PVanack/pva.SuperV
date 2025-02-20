using pva.SuperV.Engine;
using pva.SuperV.Model;
using System.ComponentModel;

namespace pva.SuperV.Api
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