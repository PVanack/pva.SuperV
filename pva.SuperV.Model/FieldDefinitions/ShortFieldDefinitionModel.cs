using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Short field definition.")]
    [ExcludeFromCodeCoverage]
    public record ShortFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] short DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(short).ToString(), ValueFormatter);
}