using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Short field definition.")]
    public record ShortFieldDefinitionModel : FieldDefinitionModel
    {
        public ShortFieldDefinitionModel(string Name) : base(Name, nameof(ShortFieldDefinitionModel))
        {
        }
    }
}