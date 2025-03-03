using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Time span field definition.")]
    public record TimeSpanFieldDefinitionModel : FieldDefinitionModel
    {
        public TimeSpanFieldDefinitionModel(string Name) : base(Name, nameof(TimeSpanFieldDefinitionModel))
        {
        }
    }
}