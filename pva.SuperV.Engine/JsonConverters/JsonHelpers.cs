using System.Text.Json;

namespace pva.SuperV.Engine.JsonConverters
{
    /// <summary>
    /// Json helpers.
    /// </summary>
    internal static class JsonHelpers
    {
        /// <summary>
        /// Gets a string property from utf reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Value of property from Json</returns>
        /// <exception cref="JsonException"></exception>
        public static string? GetStringPropertyFromUtfReader(ref Utf8JsonReader reader, string propertyName, bool readFromReader = true)
        {
            ReadTokenType(ref reader, JsonTokenType.PropertyName, readFromReader);
            ReadPropertyName(ref reader, propertyName);
            ReadTokenType(ref reader, JsonTokenType.String);
            return reader.GetString();
        }

        public static void ReadTokenType(ref Utf8JsonReader reader, JsonTokenType tokenType, bool readFromReader = true)
        {
            if (readFromReader)
            {
                reader.Read();
            }
            if (reader.TokenType != tokenType)
            {
                throw new JsonException($"Expected {tokenType} token type. Got {reader.TokenType}");
            }
        }

        public static void ReadPropertyName(ref Utf8JsonReader reader, string propertyName)
        {
            string? readPropertyName = reader.GetString();
            if (readPropertyName != propertyName)
            {
                throw new JsonException($"Expected {propertyName}. Got {readPropertyName}");
            }
        }
    }
}