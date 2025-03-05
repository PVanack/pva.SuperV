using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("String field definition.")]
    public record StringFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] string? DefaultValue)
            : FieldDefinitionModel(Name, typeof(string).ToString())
    {
    }
}