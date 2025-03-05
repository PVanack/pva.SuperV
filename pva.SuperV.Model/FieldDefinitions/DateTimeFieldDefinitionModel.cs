using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Date and time field definition.")]
    public record DateTimeFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] DateTime DefaultValue)
            : FieldDefinitionModel(Name, typeof(DateTime).ToString())
    {
    }
}