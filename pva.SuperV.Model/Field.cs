using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    public class Field<T>(T value) : IField
    {
        [JsonIgnore]
        public Type Type => typeof(T);

        [JsonIgnore]
        public virtual T Value { get; set; } = value;
    }
}