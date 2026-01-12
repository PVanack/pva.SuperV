using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Date and time field definition.")]
    [ExcludeFromCodeCoverage]
    public record DateTimeFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] DateTime DefaultValue,
            string? ValueFormatter,
            string? TopicName = "")
            : FieldDefinitionModel(Name, typeof(DateTime).ToString(), ValueFormatter, TopicName);
}