using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Double field definition.")]
    [ExcludeFromCodeCoverage]
    public record DoubleFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] double DefaultValue,
            string? ValueFormatter,
            string? TopicName = "")
            : FieldDefinitionModel(Name, typeof(double).ToString(), ValueFormatter, TopicName);
}