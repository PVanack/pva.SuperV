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
        public static string? GetStringPropertyFromUtfReader(ref Utf8JsonReader reader, string propertyName)
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