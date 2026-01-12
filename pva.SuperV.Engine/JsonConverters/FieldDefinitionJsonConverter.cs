using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.Processing;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine.JsonConverters
{
    /// <summary>
    /// Json converter for field definition
    /// </summary>
    /// <seealso cref="JsonConverter&lt;IFieldDefinition&gt;" />
    public class FieldDefinitionJsonConverter : JsonConverter<IFieldDefinition>
    {
        /// <summary>
        /// The field converters cache.
        /// </summary>
        private static readonly Dictionary<Type, dynamic> FieldConvertersCache = [];

        /// <summary>
        /// Reads and converts the JSON to type.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <exception cref="JsonException"></exception>
        public override IFieldDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.StartObject, false);
            string? fieldTypeString = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Type");
            Type? fieldType = Type.GetType(fieldTypeString!);
            string? fieldName = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Name");

            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.PropertyName);
            JsonHelpers.ReadPropertyName(ref reader, "DefaultValue");
            dynamic? defaultValue = JsonSerializer.Deserialize(ref reader, fieldType!, options);

            string? topicName = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "TopicName");

            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.PropertyName);
            JsonHelpers.ReadPropertyName(ref reader, "ValuePostChangeProcessings");
            List<IFieldValueProcessing>? fieldValueProcessings = JsonSerializer.Deserialize<List<IFieldValueProcessing>>(ref reader, options);

            FieldFormatter? fieldFormatter = null;
            reader.Read();
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                if (reader.GetString() != "Formatter")
                {
                    throw new JsonException();
                }
                fieldFormatter = JsonSerializer.Deserialize<FieldFormatter>(ref reader, options);
                reader.Read();
            }

            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.EndObject, false);

            IFieldDefinition fieldDefinition = CreateInstance(fieldType!, fieldName, defaultValue, topicName);
            fieldDefinition.ValuePostChangeProcessings = fieldValueProcessings!;
            fieldDefinition.Formatter = fieldFormatter;
            return fieldDefinition;
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The field definition.</param>
        /// <param name="options">The options.</param>
        public override void Write(Utf8JsonWriter writer, IFieldDefinition value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("Type", value.Type.ToString());
            writer.WriteString("Name", value.Name);
            dynamic actualFieldDefinition = value;
            Type fieldType = actualFieldDefinition.Type;
            if (!FieldConvertersCache.TryGetValue(fieldType, out dynamic? fieldConverter))
            {
                fieldConverter = JsonSerializerOptions.Default.GetConverter(fieldType);
                FieldConvertersCache.Add(fieldType, fieldConverter);
            }
            writer.WritePropertyName("DefaultValue");
            fieldConverter.Write(writer, actualFieldDefinition.DefaultValue, options);

            writer.WriteString("TopicName", value.TopicName);

            writer.WritePropertyName("ValuePostChangeProcessings");
            JsonSerializer.Serialize(writer, value.ValuePostChangeProcessings, options);

            if (value!.Formatter is not null)
            {
                writer.WritePropertyName("Formatter");
                JsonSerializer.Serialize(writer, value.Formatter, options);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Creates an instance for targetType's <see cref="FieldDefinition{T}"/>.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="topicName">Name of the topic.</param>
        /// <returns><see cref="IFieldDefinition"/> created instance.</returns>
        private static IFieldDefinition CreateInstance(Type targetType, string fieldName, object value, string? topicName)
        {
            var ctor = GetConstructor(targetType, targetType);
            return (IFieldDefinition)ctor.Invoke([fieldName, value, topicName]);
        }

        /// <summary>
        /// Gets the constructor for targetType's <see cref="FieldDefinition{T}"/>.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="argumentType">Type of the argument.</param>
        /// <returns>Constructor info</returns>
        /// <exception cref="InvalidOperationException">No constructor found for FieldDefinition{targetType.Name}.</exception>
        private static ConstructorInfo GetConstructor(Type targetType, Type argumentType)
        {
            return typeof(FieldDefinition<>)
                .MakeGenericType(targetType)
                .GetConstructor([typeof(string), argumentType, typeof(string)])
                ?? throw new InvalidOperationException($"No constructor found for FieldDefinition<{targetType.Name}>.");
        }
    }
}