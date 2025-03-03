using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Float field definition.")]
    public record FloatFieldDefinitionModel : FieldDefinitionModel
    {
        public FloatFieldDefinitionModel(string Name) : base(Name, nameof(FloatFieldDefinitionModel))
        {
        }
    }
}