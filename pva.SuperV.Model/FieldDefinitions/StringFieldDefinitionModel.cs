using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("String field definition.")]
    public record StringFieldDefinitionModel : FieldDefinitionModel
    {
        public StringFieldDefinitionModel(string Name) : base(Name, nameof(StringFieldDefinitionModel))
        {
        }
    }
}