using pva.SuperV.Engine.JsonConverters;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Interface for value processing of a <see cref="Field{T}"/> trigerring field. Used to allow usage in List/Dictionnaries.
    /// </summary>
    [JsonConverter(typeof(FieldValueProcessingJsonConverter))]
    public interface IFieldValueProcessing
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the constructor arguments.
        /// </summary>
        /// <value>
        /// The constructor arguments.
        /// </value>
        List<object> CtorArguments { get; set; }

        /// <summary>
        /// Gets the field definition which triggers the processing.
        /// </summary>
        /// <value>
        /// The trigerring field definition.
        /// </value>
        public IFieldDefinition? TrigerringFieldDefinition { get; set; }

        /// <summary>
        /// Builds the field value processing from the <see cref="CtorArguments"/> after deserialization.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="clazz">The clazz.</param>
        void BuildAfterDeserialization(Project project, Class clazz);
    }
}
