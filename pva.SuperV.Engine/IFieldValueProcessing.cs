using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
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
        public string? Name { get; set; }

        /// <summary>
        /// Gets the type of the trigerring field.
        /// </summary>
        /// <value>
        /// The type of the trigerring field.
        /// </value>
        Type TrigerringFieldType { get; }

        /// <summary>
        /// Gets the additional types.
        /// </summary>
        /// <value>
        /// The additional types.
        /// </value>
        List<Type> AdditionalTypes { get; }

        /// <summary>
        /// Gets or sets the constructor arguments.
        /// </summary>
        /// <value>
        /// The constructor arguments.
        /// </value>
        List<Object> CtorArguments { get; set; }

        /// <summary>
        /// Builds the field value processing from the <see cref="CtorArguments"/> after deserialization.
        /// </summary>
        /// <param name="clazz">The clazz.</param>
        void BuildAfterDeserialization(Class clazz);
    }
}
