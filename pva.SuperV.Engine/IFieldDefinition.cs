using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.JsonConverters;
using pva.SuperV.Engine.Processing;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Field definition base interface to store <see cref="FieldDefinition{T}"/> in list/dictionnaries.
    /// </summary>
    [JsonConverter(typeof(FieldDefinitionJsonConverter))]
    public interface IFieldDefinition
    {
        /// <summary>
        /// Gets or sets the type of the field definition.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type { get; init; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the field formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        FieldFormatter? Formatter { get; set; }

        /// <summary>
        /// Gets or sets the value post change processings.
        /// </summary>
        /// <value>
        /// The value post change processings.
        /// </value>
        List<IFieldValueProcessing> ValuePostChangeProcessings { get; set; }

        /// <summary>
        /// Gets or sets the name of the topic to trigger script(s) when the value changes.
        /// </summary>
        /// <value>
        /// The name of the topic.
        /// </value>
        string? TopicName { get; init; }

        /// <summary>
        /// Gets or sets the field value changed event.
        /// </summary>
        /// <value>
        /// The field value changed event.
        /// </value>
        Channel<FieldValueChangedEvent>? FieldValueChangedEventChannel { get; set; }

        /// <summary>
        /// Gets the C# code representation of the field in class.
        /// </summary>
        /// <returns>C# code</returns>
        string GetCode();

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned field definition.</returns>
        IFieldDefinition Clone();

        /// <summary>
        /// Update field definition from another field definition.
        /// </summary>
        /// <param name="fieldDefinitionUpdate">Field from which to update. Only default value and formatter are copied.</param>
        /// <param name="fieldFormatter">Formatter</param>
        void Update(IFieldDefinition fieldDefinitionUpdate, FieldFormatter? fieldFormatter);
    }
}