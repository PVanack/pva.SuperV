using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Enum formatter for integer fields. Allows to display the string representation according to a list of strings.
    /// </summary>
    /// <seealso cref="pva.SuperV.Engine.FieldFormatter" />
    public class EnumFormatter : FieldFormatter
    {
        /// <summary>
        /// The allowed types for Enum formatter.
        /// </summary>
        private static readonly HashSet<Type> EnumAllowedTypes =
            [
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong)
            ];

        /// <summary>
        /// Gets or sets the string values of enum.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public Dictionary<int, string>? Values { get; set; } = [];

        /// <summary>
        /// Gets or sets the string values of enum.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        private Dictionary<string, int>? StringsToValues { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFormatter"/> class. Used by JSON deserializer.
        /// </summary>
        public EnumFormatter() : base(EnumAllowedTypes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFormatter"/> class with values starting at 0.
        /// </summary>
        /// <param name="enumName">Name of the enum.</param>
        /// <param name="values">The string values of enum.</param>
        public EnumFormatter(string enumName, HashSet<string> values) : base(enumName, EnumAllowedTypes)
        {
            int index = 0;
            values.ForEach(value
                => Values.Add(index++, value));
            CreateStringsToValuesDictionary();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFormatter"/> class with strng values and their associated int value.
        /// </summary>
        /// <param name="enumName">Name of the enum.</param>
        /// <param name="values">The value pairs (int and string).</param>
        public EnumFormatter(string enumName, Dictionary<int, string> values) : base(enumName, EnumAllowedTypes)
        {
            Values = values;
        }


        private void CreateStringsToValuesDictionary()
        {
            Values?.ForEach(entry
                => StringsToValues?.Add(entry.Value, entry.Key));
        }
        /// <summary>
        /// Converts a value to string.
        /// </summary>
        /// <param name="value">The (int) value.</param>
        /// <returns>String representation of value. If int value is not found in <see cref="Values"/>, the int value with a question mark is returned.</returns>
        public override string? ConvertToString(dynamic? value)
        {
            if (value is null)
            {
                return null;
            }
            int longValue = (int)value;
            if (Values!.TryGetValue(longValue, out string? stringValue))
            {
                return stringValue;
            }
            return $"{longValue} ?";
        }

        public override void ConvertFromString(IField field, string? stringValue, DateTime? timestamp, QualityLevel? quality)
        {
            if (String.IsNullOrEmpty(stringValue))
            {
                throw new StringConversionException(field.FieldDefinition!.Name, stringValue, field.Type);
            }
            if (StringsToValues!.TryGetValue(stringValue, out int convertedValue))
            {
                (field switch
                {
                    Field<int> typedField => new Action(() => typedField.SetValue(convertedValue, timestamp, quality)),
                    Field<long> typedField => new Action(() => typedField.SetValue(convertedValue, timestamp, quality)),
                    Field<short> typedField => new Action(() => typedField.SetValue((short)convertedValue, timestamp, quality)),
                    Field<uint> typedField => new Action(() => typedField.SetValue((uint)convertedValue, timestamp, quality)),
                    Field<ulong> typedField => new Action(() => typedField.SetValue((ulong)convertedValue, timestamp, quality)),
                    Field<ushort> typedField => new Action(() => typedField.SetValue((ushort)convertedValue, timestamp, quality)),
                    _ => new Action(() => throw new UnhandledFieldTypeException(field.FieldDefinition!.Name, field.Type))
                })();
            }
        }

    }
}