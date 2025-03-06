using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Unsigned long field definition.")]
    public record UlongFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] ulong DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(ulong).ToString(), ValueFormatter)
    {
    }
}
