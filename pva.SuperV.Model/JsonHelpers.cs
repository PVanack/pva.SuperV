using System.Text.Json;

namespace pva.SuperV.Model
{
    internal static class JsonHelpers
    {
        public static String? GetStringPropertyFromUtfReader(ref Utf8JsonReader reader, string propertyName)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string? readPropertyName = reader.GetString();
            if (readPropertyName != propertyName)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }
            return reader.GetString();
        }
    }
}