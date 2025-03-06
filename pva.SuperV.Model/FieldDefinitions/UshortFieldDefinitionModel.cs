using System.ComponentModel;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Unsigned short field definition.")]
    public record UshortFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] ushort DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(ushort).ToString(), ValueFormatter)
    {
    }
}