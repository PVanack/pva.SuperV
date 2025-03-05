using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Unsigned int field definition.")]
    public record UintFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] uint DefaultValue)
            : FieldDefinitionModel(Name, typeof(uint).ToString())
    {
    }
}
