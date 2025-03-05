using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Short field definition.")]
    public record ShortFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] short DefaultValue)
            : FieldDefinitionModel(Name, typeof(short).ToString())
    {
    }
}