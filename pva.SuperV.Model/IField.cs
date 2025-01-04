namespace pva.SuperV.Model
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
        /// Gets or sets the field definition associated with field.
        /// </summary>
        /// <value>
        /// The field definition.
        /// </value>
        IFieldDefinition? FieldDefinition { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>String representation of field's value.</returns>
        string? ToString();
    }
}