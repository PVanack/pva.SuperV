using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    public class FieldDefinitionJsonConverter : JsonConverter<IFieldDefinition>
    {
        private static readonly Dictionary<Type, dynamic> fieldConverters = [];

        public override IFieldDefinition? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            string? fieldTypeString = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Type");

            String? fieldName = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Name");

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string? readPropertyName = reader.GetString();
            if (readPropertyName != "DefaultValue")
            {
                throw new JsonException();
            }
            reader.Read();
            reader.GetByte();
            Type fieldType = Type.GetType(fieldTypeString);
            dynamic defaultValue = JsonSerializer.Deserialize(ref reader, fieldType, options);
            dynamic fieldDefinition = CreateInstance(fieldType, fieldName, defaultValue);

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }
            return fieldDefinition;
        }

        public override void Write(Utf8JsonWriter writer, IFieldDefinition fieldDefinition, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("Type", fieldDefinition.Type.ToString());
            writer.WriteString("Name", fieldDefinition.Name);
            dynamic actualFieldDefinition = fieldDefinition;
            Type fieldType = actualFieldDefinition.Type;
            if (!fieldConverters.TryGetValue(fieldType, out dynamic fieldConverter))
            {
                fieldConverter =
                    JsonSerializerOptions.Default.GetConverter(fieldType);
                fieldConverters.Add(fieldType, fieldConverter);
            }
            writer.WritePropertyName("DefaultValue");
            fieldConverter.Write(writer, actualFieldDefinition.DefaultValue, options);

            writer.WriteEndObject();
        }

        private static ConstructorInfo GetConstructor(Type targetType, Type argumentType)
        {
            return typeof(FieldDefinition<>)
                .MakeGenericType(targetType).
                GetConstructor([typeof(string), argumentType])
                ?? throw new InvalidOperationException($"No constructor found for FieldDefinition<{targetType.Name}>.");
        }

        private static IFieldDefinition CreateInstance(Type targetType, string fieldName, object value)
        {
            var ctor = GetConstructor(targetType, targetType);
            return (IFieldDefinition)ctor.Invoke([fieldName, value]);
        }
    }
}