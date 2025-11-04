using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Boolen field definition.")]
    [ExcludeFromCodeCoverage]
    public record IntFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] int DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(int).ToString(), ValueFormatter);
}
