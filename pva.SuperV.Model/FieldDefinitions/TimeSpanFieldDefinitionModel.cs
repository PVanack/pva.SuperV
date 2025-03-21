using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldDefinitions
{
    [Description("Time span field definition.")]
    [ExcludeFromCodeCoverage]
    public record TimeSpanFieldDefinitionModel(
            string Name,
            [property: Description("Default value")] TimeSpan DefaultValue,
            string? ValueFormatter)
            : FieldDefinitionModel(Name, typeof(TimeSpan).ToString(), ValueFormatter)
    {
    }
}