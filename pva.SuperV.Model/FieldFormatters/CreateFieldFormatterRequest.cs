using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldFormatters
{
    [Description("Field formatter creation request")]
    [ExcludeFromCodeCoverage]

    public record CreateFieldFormatterRequest(
        [property:Description("Definition of field formatter")]
        FieldFormatterModel FieldFormatter);
}
