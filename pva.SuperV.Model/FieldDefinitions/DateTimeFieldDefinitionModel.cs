using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Date and time field definition.")]
    public record DateTimeFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] DateTime DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(DateTime).ToString(), ValueFormatter)
    {
    }
}