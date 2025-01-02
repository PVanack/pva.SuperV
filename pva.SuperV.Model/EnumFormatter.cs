using pva.Helpers.Extensions;

namespace pva.SuperV.Model
{
    /// <summary>
    /// Enum formatter for integer fields. Allows to display the string representation according to a list of strings.
    /// </summary>
    /// <seealso cref="pva.SuperV.Model.FieldFormatter" />
    public class EnumFormatter : FieldFormatter
    {
        /// <summary>
        /// The allowed types for Enum formatter.
        /// </summary>
        private static readonly HashSet<Type> allowedTypes = [typeof(int), typeof(long)];

        /// <summary>
        /// Gets or sets the string values of enum.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public Dictionary<int, string>? Values { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFormatter"/> class. Used by JSON deserializer.
        /// </summary>
        public EnumFormatter() : base(allowedTypes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFormatter"/> class with values starting at 0.
        /// </summary>
        /// <param name="enumName">Name of the enum.</param>
        /// <param name="values">The string values of enum.</param>
        public EnumFormatter(string enumName, HashSet<string> values) : base(enumName, allowedTypes)
        {
            int index = 0;
            values.ForEach(value => Values.Add(index++, value));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFormatter"/> class with strng values and their associated int value.
        /// </summary>
        /// <param name="enumName">Name of the enum.</param>
        /// <param name="values">The value pairs (int and string).</param>
        public EnumFormatter(string enumName, Dictionary<int, string> values) : base(enumName, allowedTypes)
        {
            Values = values;
        }

        /// <summary>
        /// Converts a value to string.
        /// </summary>
        /// <param name="value">The (int) value.</param>
        /// <returns>String representation of value. If int value is not found in <see cref="Values", the int value with a question mark is returned./></returns>
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
    }
}