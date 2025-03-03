using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Unsigned short field definition.")]
    public record UshortFieldDefinitionModel : FieldDefinitionModel
    {
        public UshortFieldDefinitionModel(string Name) : base(Name, nameof(UshortFieldDefinitionModel))
        {
        }
    }
}