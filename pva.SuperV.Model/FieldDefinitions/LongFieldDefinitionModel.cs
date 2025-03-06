using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Long field definition.")]
    public record LongFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] long DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(long).ToString(), ValueFormatter)
    {
    }
}
