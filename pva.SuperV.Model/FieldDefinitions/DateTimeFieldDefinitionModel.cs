using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Date and time field definition.")]
    public record DateTimeFieldDefinitionModel : FieldDefinitionModel
    {
        public DateTimeFieldDefinitionModel(string Name) : base(Name, nameof(DateTimeFieldDefinitionModel))
        {
        }
    }
}