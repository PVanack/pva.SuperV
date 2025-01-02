using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    public class Field<T>(T value) : IField
    {
        [JsonIgnore]
        public Type Type => typeof(T);

        public IFieldDefinition? FieldDefinition { get; set; }

        [JsonIgnore]
        public virtual T Value { get; set; } = value;

        public override string? ToString()
        {
            return FieldDefinition?.Formatter?.ConvertToString(Value) ??
                Value?.ToString();
        }
    }
}