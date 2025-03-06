using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Double field definition.")]
    public record DoubleFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] double DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(double).ToString(), ValueFormatter)
    {
    }
}