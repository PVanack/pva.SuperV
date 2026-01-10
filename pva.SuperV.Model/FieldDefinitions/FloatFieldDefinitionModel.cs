using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Float field definition.")]
    [ExcludeFromCodeCoverage]
    public record FloatFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] float DefaultValue,
            string? ValueFormatter,
            string? TopicName = "")
            : FieldDefinitionModel(Name, typeof(float).ToString(), ValueFormatter, TopicName);
}