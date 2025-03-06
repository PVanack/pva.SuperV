using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Float field definition.")]
    public record FloatFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] float DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(float).ToString(), ValueFormatter)
    {
    }
}