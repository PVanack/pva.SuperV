using pva.SuperV.Engine.Processing;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine.JsonConverters
{
    /// <summary>
    /// Json converter for field value processing
    /// </summary>
    /// <seealso cref="JsonConverter{IFieldValueProcessing}" />
    public class FieldValueProcessingJsonConverter : JsonConverter<IFieldValueProcessing>
    {
        /// <summary>
        /// The field converters cache.
        /// </summary>
        private static readonly Dictionary<Type, dynamic> ArgConvertersCache = [];

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
        public override IFieldValueProcessing Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.StartObject, false);

            string? fieldValueProcessingTypeString = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Type");
            string? fieldValueProcessingName = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Name");

            List<object> ctorArguments = ReadParameters(ref reader, options);
            Type? fieldType = Type.GetType(fieldValueProcessingTypeString!);

            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.EndObject);

            IFieldValueProcessing fieldValueProcessing = CreateInstance(fieldType!);
            fieldValueProcessing.Name = fieldValueProcessingName!;
            fieldValueProcessing.CtorArguments = ctorArguments;
            return fieldValueProcessing;
        }

        private static List<object> ReadParameters(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            List<object> ctorArguments = [];
            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.PropertyName);
            JsonHelpers.ReadPropertyName(ref reader, "Params");

            JsonHelpers.ReadTokenType(ref reader, JsonTokenType.StartArray);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return ctorArguments;
                }
                JsonHelpers.ReadTokenType(ref reader, JsonTokenType.StartObject, false);
                string? paramTypeString = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Type");

                JsonHelpers.ReadTokenType(ref reader, JsonTokenType.PropertyName);
                JsonHelpers.ReadPropertyName(ref reader, "Value");

                reader.Read();
                Type? paramType = Type.GetType(paramTypeString!);
                dynamic? argValue = JsonSerializer.Deserialize(ref reader, paramType!, options);
                JsonHelpers.ReadTokenType(ref reader, JsonTokenType.EndObject);
                ctorArguments.Add(argValue);
            }
            return ctorArguments;
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="fieldValueProcessing">The field processing.</param>
        /// <param name="options">The options.</param>
        public override void Write(Utf8JsonWriter writer, IFieldValueProcessing fieldValueProcessing, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Type", fieldValueProcessing.GetType().ToString());
            writer.WriteString("Name", fieldValueProcessing.Name);
            WriteCtorParameters(writer, fieldValueProcessing, options);
            writer.WriteEndObject();
        }

        private static void WriteCtorParameters(Utf8JsonWriter writer, IFieldValueProcessing fieldValueProcessing, JsonSerializerOptions options)
        {
            writer.WriteStartArray("Params");
            fieldValueProcessing.CtorArguments.ForEach(arg =>
                {
                    Type argType = arg.GetType();
                    writer.WriteStartObject();
                    writer.WriteString("Type", argType.ToString());
                    if (!ArgConvertersCache.TryGetValue(argType, out dynamic? argConverter))
                    {
                        argConverter = JsonSerializerOptions.Default.GetConverter(argType);
                        ArgConvertersCache.Add(argType, argConverter);
                    }
                    writer.WritePropertyName("Value");
                    JsonSerializer.Serialize(writer, arg, options);
                    writer.WriteEndObject();
                });
            writer.WriteEndArray();
        }

        /// <summary>
        /// Creates an instance for targetType's <see cref="FieldValueProcessing{T}"/>.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns><see cref="IFieldValueProcessing"/> created instance.</returns>
        private static IFieldValueProcessing CreateInstance(Type targetType)
        {
            var ctor = GetConstructor(targetType);
            return (IFieldValueProcessing)ctor.Invoke([]);
        }

        /// <summary>
        /// Gets the constructor for targetType's <see cref="FieldValueProcessing{T}"/>.
        /// </summary>
        /// <param name="fieldProcesingType">Type of the target.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No constructor found for FieldValueProcessing{targetType.Name}.</exception>
        private static ConstructorInfo GetConstructor(Type fieldProcesingType)
        {
            return fieldProcesingType
                .GetConstructor([])
                ?? throw new InvalidOperationException($"No constructor found for {fieldProcesingType.Name}.");
        }
    }
}