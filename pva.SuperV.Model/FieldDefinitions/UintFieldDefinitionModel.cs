using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Unsigned int field definition.")]
    [ExcludeFromCodeCoverage]
    public record UintFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] uint DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(uint).ToString(), ValueFormatter);
}
