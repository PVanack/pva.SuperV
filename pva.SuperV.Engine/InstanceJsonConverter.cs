using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Json converter for an instance.
    /// </summary>
    /// <seealso cref="System.Text.Json.Serialization.JsonConverter&lt;pva.SuperV.Model.IInstance&gt;" />
    public class InstanceJsonConverter : JsonConverter<IInstance>
    {
        /// <summary>
        /// The field converters
        /// </summary>
        private static readonly Dictionary<Type, dynamic> fieldConverters = [];

        /// <summary>
        /// Gets or sets the loaded project. Set before deserializing.
        /// </summary>
        /// <value>
        /// The loaded project.
        /// </value>
        public static RunnableProject LoadedProject { get; set; } = null!;

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
        /// <exception cref="pva.SuperV.Model.Exceptions.InstanceCreationException"></exception>
        public override IInstance? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            string? instanceClass = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Class");
            String? instanceName = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Name");

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            try
            {
                IInstance? instance = LoadedProject.CreateInstance(instanceClass!, instanceName!);

                reader.Read();
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    throw new JsonException();
                }

                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType != JsonTokenType.StartObject)
                    {
                        throw new JsonException();
                    }
                    string? fieldTypeString = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Type");
                    String? fieldName = JsonHelpers.GetStringPropertyFromUtfReader(ref reader, "Name");

                    reader.Read();
                    string? readPropertyName = reader.GetString();
                    if (readPropertyName != "Value")
                    {
                        throw new JsonException();
                    }

                    Type? fieldType = Type.GetType(fieldTypeString!);
                    dynamic? fieldValue = JsonSerializer.Deserialize(ref reader, fieldType!, options);
                    IField? field = (instance as Instance)?.GetField(fieldName!);
                    dynamic? dynamicField = field as dynamic;
                    dynamicField!.Value = fieldValue;

                    reader.Read();
                    if (reader.TokenType != JsonTokenType.EndObject)
                    {
                        throw new JsonException();
                    }
                }

                reader.Read();
                if (reader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException();
                }
                return instance;
            }
            catch (Exception ex)
            {
                throw new InstanceCreationException(instanceName!, instanceClass!, ex);
            }
        }

        /// <summary>
        /// Writes a specified value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, IInstance value, JsonSerializerOptions options)
        {
            Instance instance = (Instance)value;
            writer.WriteStartObject();

            writer.WriteString("Class", instance.Class.Name);
            writer.WriteString("Name", instance.Name);
            writer.WriteStartArray("Fields");
            instance.Fields.ForEach((k, v) =>
            {
                writer.WriteStartObject();
                Type fieldType = v.Type;
                writer.WriteString("Type", fieldType.ToString());
                writer.WriteString("Name", k);
                writer.WritePropertyName("Value");
                if (!fieldConverters.TryGetValue(fieldType!, out dynamic? fieldConverter))
                {
                    fieldConverter =
                        JsonSerializerOptions.Default.GetConverter(fieldType);
                    fieldConverters.Add(fieldType, fieldConverter);
                }
                fieldConverter.Write(writer, ((dynamic)v).Value, options);
                writer.WriteEndObject();
            });
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}