using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Boolen field definition.")]
    public record IntFieldDefinitionModel : FieldDefinitionModel
    {
        public IntFieldDefinitionModel(string Name) : base(Name, nameof(IntFieldDefinitionModel))
        {
        }
    }
}
