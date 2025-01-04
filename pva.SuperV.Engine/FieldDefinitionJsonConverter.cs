using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Json converter for field definition
    /// </summary>
    /// <seealso cref="System.Text.Json.Serialization.JsonConverter&lt;pva.SuperV.Engine.IFieldDefinition&gt;" />
    public class FieldDefinitionJsonConverter : JsonConverter<IFieldDefinition>
    {
        /// <summary>
        /// The field converters cache.
        /// </summary>
        private static readonly Dictionary<Type, dynamic> fieldConvertersCache = [];

        /// <summary>
        /// Reads and converts the JSON to type <typeparamref name="T" />.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="System.Text.Json.JsonException"></exception>
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
            Type? fieldType = Type.GetType(fieldTypeString!);
            dynamic? defaultValue = JsonSerializer.Deserialize(ref reader, fieldType!, options);
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            readPropertyName = reader.GetString();
            if (readPropertyName != "ValuePostChangeProcessings")
            {
                throw new JsonException();
            }
            List<IFieldValueProcessing>? fieldValueProcessings = JsonSerializer.Deserialize<List<IFieldValueProcessing>>(ref reader, options);
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            IFieldDefinition fieldDefinition = CreateInstance(fieldType!, fieldName, defaultValue);
            fieldDefinition.ValuePostChangeProcessings = fieldValueProcessings!;
            return fieldDefinition;
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="fieldDefinition">The field definition.</param>
        /// <param name="options">The options.</param>
        public override void Write(Utf8JsonWriter writer, IFieldDefinition fieldDefinition, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("Type", fieldDefinition.Type.ToString());
            writer.WriteString("Name", fieldDefinition.Name);
            dynamic actualFieldDefinition = fieldDefinition;
            Type fieldType = actualFieldDefinition.Type;
            if (!fieldConvertersCache.TryGetValue(fieldType, out dynamic? fieldConverter))
            {
                fieldConverter = JsonSerializerOptions.Default.GetConverter(fieldType);
                fieldConvertersCache.Add(fieldType, fieldConverter);
            }
            writer.WritePropertyName("DefaultValue");
            fieldConverter.Write(writer, actualFieldDefinition.DefaultValue, options);

            writer.WritePropertyName("ValuePostChangeProcessings");
            JsonSerializer.Serialize<List<IFieldValueProcessing>>(writer, fieldDefinition!.ValuePostChangeProcessings, options);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Creates an instance for targetType's <see cref="FieldDefinition{T}"/>.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns><see cref="IFieldDefinition"/> created instance.</returns>
        private static IFieldDefinition CreateInstance(Type targetType, string fieldName, object value)
        {
            var ctor = GetConstructor(targetType, targetType);
            return (IFieldDefinition)ctor.Invoke([fieldName, value]);
        }

        /// <summary>
        /// Gets the constructor for targetType's <see cref="FieldDefinition{T}"/>.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="argumentType">Type of the argument.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">No constructor found for FieldDefinition<{targetType.Name}>.</exception>
        private static ConstructorInfo GetConstructor(Type targetType, Type argumentType)
        {
            return typeof(FieldDefinition<>)
                .MakeGenericType(targetType)
                .GetConstructor([typeof(string), argumentType])
                ?? throw new InvalidOperationException($"No constructor found for FieldDefinition<{targetType.Name}>.");
        }
    }
}