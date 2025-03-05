using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Time span field definition.")]
    public record TimeSpanFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] TimeSpan DefaultValue)
            : FieldDefinitionModel(Name, typeof(TimeSpan).ToString())
    {
    }
}