using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Unsigned long field definition.")]
    public record UlongFieldDefinitionModel : FieldDefinitionModel
    {
        public UlongFieldDefinitionModel(string Name) : base(Name, nameof(UlongFieldDefinitionModel))
        {
        }
    }
}
