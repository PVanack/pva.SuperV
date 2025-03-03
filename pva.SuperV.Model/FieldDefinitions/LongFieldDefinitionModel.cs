using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Long field definition.")]
    public record LongFieldDefinitionModel : FieldDefinitionModel
    {
        public LongFieldDefinitionModel(string Name) : base(Name, nameof(LongFieldDefinitionModel))
        {
        }
    }
}
