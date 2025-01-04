using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Field of an <see cref="Instance"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="pva.SuperV.Engine.IField" />
    public class Field<T>(T value) : IField
    {
        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        [JsonIgnore]
        public Type Type => typeof(T);

        /// <summary>
        /// Gets or sets the instance to which the field belongs.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        [JsonIgnore]
        public Instance? Instance { get; set; }

        /// <summary>
        /// Gets or sets the field definition associated with field.
        /// </summary>
        /// <value>
        /// The field definition.
        /// </value>
        public IFieldDefinition? FieldDefinition { get; set; }

        private T _value = value;

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonIgnore]
        public virtual T Value
        {
            get => _value;
            set
            {
                T previousValue = _value;
                _value = value;
                FieldDefinition?.ValuePostChangeProcessings.ForEach(
                    processing =>
                        {
                            FieldValueProcessing<T>? fieldValueProcessing = processing as FieldValueProcessing<T>;
                            fieldValueProcessing!.ProcessValue(Instance!, this, !previousValue!.Equals(value), previousValue, value);
                        });
            }
        }

        /// <summary>
        /// Converts field's value to string. If a field formatter is defined in field definition, it's used.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string? ToString()
        {
            return FieldDefinition?.Formatter?.ConvertToString(Value) ??
                Value?.ToString();
        }
    }
}