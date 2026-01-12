namespace pva.SuperV.Engine
{
    /// <summary>
    /// Used to store <see cref="Field{T}"/> in fields dictonnary of <see cref="Instance"/>
    /// </summary>
    public interface IField
    {
        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type { get; }

        /// <summary>
        /// Gets or sets the instance to which the field belongs.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        Instance? Instance { get; set; }

        /// <summary>
        /// Gets or sets the field definition associated with field.
        /// </summary>
        /// <value>
        /// The field definition.
        /// </value>
        IFieldDefinition? FieldDefinition { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of value.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        DateTime? Timestamp { get; }

        /// <summary>
        /// Gets or sets the quality level of the value.
        /// </summary>
        /// <value>
        /// The quality <see cref="QualityLevel"/>.
        /// </value>
        QualityLevel? Quality { get; }

        /// <summary>
        /// Converts field's value to string. If a field formatter is defined in field definition, it's used.
        /// </summary>
        /// <returns>String representation of field's value.</returns>
        string? ToString();
    }
}