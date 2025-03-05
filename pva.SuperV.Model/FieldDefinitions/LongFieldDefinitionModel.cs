using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Long field definition.")]
    public record LongFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] long DefaultValue)
            : FieldDefinitionModel(Name, typeof(long).ToString())
    {
    }
}
