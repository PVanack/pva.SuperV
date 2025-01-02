using pva.Helpers.Extensions;

namespace pva.SuperV.Model
{
    public class EnumFormatter : FieldFormatter
    {
        private static readonly HashSet<Type> allowedTypes = [typeof(int), typeof(long)];

        public Dictionary<int, string>? Values { get; set; } = [];

        public EnumFormatter() : base(allowedTypes)
        {
        }

        public EnumFormatter(string enumName, HashSet<string> values) : base(enumName, allowedTypes)
        {
            int index = 0;
            values.ForEach(value => Values.Add(index++, value));
        }

        public EnumFormatter(string enumName, Dictionary<int, string> values) : base(enumName, allowedTypes)
        {
            Values = values;
        }

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
            return $"{longValue}?";
        }
    }
}