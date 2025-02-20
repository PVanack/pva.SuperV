using pva.SuperV.Api;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    [Description("Field formatter")]
    [JsonDerivedType(typeof(EnumFormatterModel), typeDiscriminator: nameof(EnumFormatterModel))]
    public abstract record FieldFormatterModel(
        [property: Description("Name of the formatter.")]
        string Name,
        [property: Description("Type of the formatter.")]
        string FormatterType)
    {
    }
}
