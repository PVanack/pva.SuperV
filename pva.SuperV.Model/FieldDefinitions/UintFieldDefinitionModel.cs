using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Unsigned int field definition.")]
    public record UintFieldDefinitionModel : FieldDefinitionModel
    {
        public UintFieldDefinitionModel(string Name) : base(Name, nameof(UintFieldDefinitionModel))
        {
        }
    }
}
