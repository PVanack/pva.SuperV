using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Unsigned short field definition.")]
    [ExcludeFromCodeCoverage]
    public record UshortFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] ushort DefaultValue,
            string? ValueFormatter,
            string? TopicName = "")
            : FieldDefinitionModel(Name, typeof(ushort).ToString(), ValueFormatter, TopicName);
}