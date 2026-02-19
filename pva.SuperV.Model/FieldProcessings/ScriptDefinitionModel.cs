using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.FieldProcessings
{
    [Description("Script reacting to a field value change.")]
    [ExcludeFromCodeCoverage]
    public record ScriptDefinitionModel(
        [property: Description("Script name.")]
        string Name,
        [property: Description("Name of the topic on which the script registers.")]
        string TopicName,
        [property: Description("Source code of the script.")]
        string Source);
}
