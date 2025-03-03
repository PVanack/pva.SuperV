using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Boolean field definition.")]
    public record BoolFieldDefinitionModel : FieldDefinitionModel
    {
        public BoolFieldDefinitionModel(string Name) : base(Name, nameof(BoolFieldDefinitionModel))
        {
        }
    }
}