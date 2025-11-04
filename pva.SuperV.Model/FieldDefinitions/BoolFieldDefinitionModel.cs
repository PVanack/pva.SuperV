using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Boolean field definition.")]
    [ExcludeFromCodeCoverage]
    public record BoolFieldDefinitionModel(
        string Name,
        [property: Description("Default value")] bool DefaultValue,
        string? ValueFormatter)
        : FieldDefinitionModel(Name, typeof(bool).ToString(), ValueFormatter);
}