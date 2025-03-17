using System.Text.Json;

namespace pva.Helpers.Extensions
{
    public static class JSonElementExtensions
    {
        public static bool IsEqualTo(this JsonElement jsonElement, JsonElement other)
        {
            return jsonElement.ValueKind == other.ValueKind &&
                jsonElement.GetRawText().Equals(other.GetRawText());
        }
    }
}
