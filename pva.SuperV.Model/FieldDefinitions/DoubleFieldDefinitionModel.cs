using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Double field definition.")]
    public record DoubleFieldDefinitionModel : FieldDefinitionModel
    {
        public DoubleFieldDefinitionModel(string Name) : base(Name, nameof(DoubleFieldDefinitionModel))
        {
        }
    }
}