using pva.Helpers;
using pva.SuperV.Model.Exceptions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    public class InstanceJsonConverter : JsonConverter<IInstance>
    {
        public static RunnableProject LoadedProject { get; set; }
        private static Dictionary<Type, dynamic> fieldConverters = new();

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
                IInstance instance = LoadedProject.CreateInstance(instanceClass, instanceName);

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
                    dynamic fieldValue = JsonSerializer.Deserialize(ref reader, Type.GetType(fieldTypeString), options);
                    ((dynamic)((Instance)instance).GetField(fieldName)).Value = fieldValue;

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
                throw new InstanceCreationException(instanceName, instanceClass, ex);
            }
        }

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
                dynamic fieldConverter;
                if (!fieldConverters.TryGetValue(fieldType!, out fieldConverter))
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
