using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Boolean field definition.")]
    public record BoolFieldDefinitionModel(
        string Name,
        [property: Description("Default value")] bool DefaultValue,
        string? ValueFormatter)
        : FieldDefinitionModel(Name, typeof(bool).ToString(), ValueFormatter)
    {
    }
}