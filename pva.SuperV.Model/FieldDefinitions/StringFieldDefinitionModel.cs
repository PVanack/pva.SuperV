using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("String field definition.")]
    [ExcludeFromCodeCoverage]
    public record StringFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] string? DefaultValue,
            string? ValueFormatter,
            string? TopicName = "")
            : FieldDefinitionModel(Name, typeof(string).ToString(), ValueFormatter, TopicName);
}