using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using System.Globalization;
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
        public void SetValue(T newValue, DateTime timestamp)
        {
            SetValue(newValue, timestamp, QualityLevel.Good);
        }

        /// <summary>
        /// Sets the value and associated quality with a <see cref="DateTime.UtcNow"/> timestamp .
        /// </summary>
        /// <param name="newValue">The value to be set.</param>
        /// <param name="quality">The quality of value.</param>
        public void SetValue(T newValue, QualityLevel quality)
        {
            SetValue(newValue, DateTime.UtcNow, quality);
        }

        /// <summary>
        /// Sets the value and associated timestamp with a <see cref="QualityLevel.Good"/>.
        /// </summary>
        /// <param name="newValue">The value to be set.</param>
        /// <param name="timestamp">The timestamp of value.</param>
        /// <param name="quality">The quality of value.</param>
        public void SetValue(T newValue, DateTime timestamp, QualityLevel quality)
        {
            T previousValue = _value;
            SetValueInternal(newValue, timestamp, quality);
            ProcessNewValue(!previousValue!.Equals(newValue), newValue, previousValue!);
        }

        public void SetValueInternal(T newValue, DateTime timestamp, QualityLevel quality)
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

        public static void SetValue<T1>(IField field, T1 value, DateTime timestamp, QualityLevel quality)
        {
            if (field.Type == typeof(T1))
            {
                ((Field<T1>)field).SetValue(value, timestamp, quality);
            }
            else if (value is string stringValue)
            {
                (field switch
                {
                    Field<bool> typedField => new Action(() => typedField.SetValue(ConvertToBool(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<DateTime> typedField => new Action(() => typedField.SetValue(ConvertToDateTime(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<double> typedField => new Action(() => typedField.SetValue(ConvertToDouble(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<float> typedField => new Action(() => typedField.SetValue(ConvertToFloat(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<int> typedField => new Action(() => typedField.SetValue(ConvertToInt(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<long> typedField => new Action(() => typedField.SetValue(ConvertToLong(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<short> typedField => new Action(() => typedField.SetValue(ConvertToShort(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<TimeSpan> typedField => new Action(() => typedField.SetValue(ConvertToTimeSpan(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<uint> typedField => new Action(() => typedField.SetValue(ConvertToUint(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<ulong> typedField => new Action(() => typedField.SetValue(ConvertToUlong(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    Field<ushort> typedField => new Action(() => typedField.SetValue(ConvertToUshort(field.FieldDefinition!.Name, stringValue), timestamp, quality)),
                    _ => new Action(() => throw new UnhandledFieldTypeException(field.FieldDefinition!.Name, field.Type))
                })();
            }
            throw new UnhandledFieldTypeException(field.FieldDefinition!.Name, field.Type);
        }

        public static bool ConvertToBool(string fieldName, string stringValue)
             => Boolean.TryParse(stringValue, out bool result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(bool));

        public static DateTime ConvertToDateTime(string fieldName, string stringValue)
             => DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, out DateTime result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(DateTime));

        public static double ConvertToDouble(string fieldName, string stringValue)
             => double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(double));

        public static float ConvertToFloat(string fieldName, string stringValue)
             => float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(float));

        public static int ConvertToInt(string fieldName, string stringValue)
             => int.TryParse(stringValue, out int result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(int));

        public static long ConvertToLong(string fieldName, string stringValue)
             => long.TryParse(stringValue, out long result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(long));

        public static short ConvertToShort(string fieldName, string stringValue)
             => short.TryParse(stringValue, out short result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(short));

        public static TimeSpan ConvertToTimeSpan(string fieldName, string stringValue)
             => TimeSpan.TryParse(stringValue, CultureInfo.InvariantCulture, out TimeSpan result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(TimeSpan));

        public static uint ConvertToUint(string fieldName, string stringValue)
             => uint.TryParse(stringValue, out uint result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(uint));

        public static ulong ConvertToUlong(string fieldName, string stringValue)
             => ulong.TryParse(stringValue, out ulong result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(ulong));

        public static ushort ConvertToUshort(string fieldName, string stringValue)
             => ushort.TryParse(stringValue, out ushort result) ? result : throw new StringConversionException(fieldName, stringValue, typeof(ushort));
    }
}