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
            string? fieldName = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Name");

            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.PropertyName);
            JsonHelpers.ReadPropertyName(ref reader, "DefaultValue");

            Type? fieldType = Type.GetType(fieldTypeString!);
            dynamic? defaultValue = JsonSerializer.Deserialize(ref reader, fieldType!, options);
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

            IFieldDefinition fieldDefinition = CreateInstance(fieldType!, fieldName, defaultValue);
            fieldDefinition.ValuePostChangeProcessings = fieldValueProcessings!;
            fieldDefinition.Formatter = fieldFormatter;
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
            if (!FieldConvertersCache.TryGetValue(fieldType, out dynamic? fieldConverter))
            {
                fieldConverter = JsonSerializerOptions.Default.GetConverter(fieldType);
                FieldConvertersCache.Add(fieldType, fieldConverter);
            }
            writer.WritePropertyName("DefaultValue");
            fieldConverter.Write(writer, actualFieldDefinition.DefaultValue, options);

            writer.WritePropertyName("ValuePostChangeProcessings");
            JsonSerializer.Serialize(writer, fieldDefinition.ValuePostChangeProcessings, options);

            if (fieldDefinition!.Formatter is not null)
            {
                writer.WritePropertyName("Formatter");
                JsonSerializer.Serialize(writer, fieldDefinition.Formatter, options);
            }
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
        /// <exception cref="InvalidOperationException">No constructor found for FieldDefinition{targetType.Name}.</exception>
        private static ConstructorInfo GetConstructor(Type targetType, Type argumentType)
        {
            return typeof(FieldDefinition<>)
                .MakeGenericType(targetType)
                .GetConstructor([typeof(string), argumentType])
                ?? throw new InvalidOperationException($"No constructor found for FieldDefinition<{targetType.Name}>.");
        }
    }
}