using pva.SuperV.Engine.Processing;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Field of an <see cref="Instance"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="pva.SuperV.Engine.IField" />
    public class Field<T> : IField
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

        private T _value;

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Bug", "S4275:Getters and setters should access the expected fields", Justification = "It sets the _value in SetValueInternal()")]
        public virtual T Value
        {
            get => _value;
            set => SetValue(value);
        }

        private DateTime? _timestamp;
        /// <summary>
        /// Gets or sets the timestamp of value.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime? Timestamp
        {
            get
            {
                _timestamp ??= DateTime.Now;
                return _timestamp;
            }
        }

        private QualityLevel? _quality;
        /// <summary>
        /// Gets or sets the quality level of the value.
        /// </summary>
        /// <value>
        /// The quality <see cref="QualityLevel" />.
        /// </value>
        public QualityLevel? Quality
        {
            get
            {
                _quality ??= QualityLevel.Good;
                return _quality;
            }
        }

        public Field(T value)
        {
            _value = value;
            _timestamp ??= DateTime.Now;
            _quality ??= QualityLevel.Good;
        }

        /// <summary>
        /// Sets the value with <see cref="DateTime.UtcNow"/> timestamp and a <see cref="QualityLevel.Good"/>.
        /// </summary>
        /// <param name="newValue">The value to be set.</param>
        public void SetValue(T newValue)
        {
            SetValue(newValue, DateTime.UtcNow, QualityLevel.Good);
        }

        /// <summary>
        /// Sets the value and associated timestamp with a <see cref="QualityLevel.Good"/>.
        /// </summary>
        /// <param name="newValue">The value to be set.</param>
        /// <param name="timestamp">The timestamp of value.</param>
        /// <param name="quality">The quality of value.</param>
        public void SetValue(T newValue, DateTime? timestamp, QualityLevel? quality)
        {
            T previousValue = _value;
            SetValueInternal(newValue, timestamp ?? DateTime.Now, quality ?? QualityLevel.Good);
            bool valueChanged = (EqualityComparer<T?>.Default.Equals(previousValue, default) && (previousValue as object) != (newValue as object)) ||
                !previousValue!.Equals(newValue);
            ProcessNewValue(valueChanged, newValue, previousValue!);
        }

        public void SetValueInternal(T newValue, DateTime? timestamp, QualityLevel? quality)
        {
            _timestamp = timestamp;
            _quality = quality;
            _value = newValue;
        }

        private void ProcessNewValue(bool hasValueChanged, T newValue, T previousValue)
        {
            FieldDefinition?.ValuePostChangeProcessings.ForEach(
                processing =>
                {
                    FieldValueProcessing<T>? fieldValueProcessing = processing as FieldValueProcessing<T>;
                    fieldValueProcessing!.ProcessValue(Instance!, this, hasValueChanged, previousValue, newValue);
                });
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