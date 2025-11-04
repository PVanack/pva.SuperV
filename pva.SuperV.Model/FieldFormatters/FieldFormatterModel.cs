using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model.FieldFormatters
{
    [Description("Field formatter")]
    [ExcludeFromCodeCoverage]
    [JsonDerivedType(typeof(EnumFormatterModel), typeDiscriminator: nameof(EnumFormatterModel))]
    public abstract record FieldFormatterModel(
        [property: Description("Name of the formatter.")]
        [Required(AllowEmptyStrings = false)]
        string Name,
        [property: Description("Type of the formatter.")]
        string FormatterType);
}
