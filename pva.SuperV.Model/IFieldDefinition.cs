using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    [JsonConverter(typeof(FieldDefinitionJsonConverter))]
    public interface IFieldDefinition
    {
        Type Type { get; set; }
        string? Name { get; set; }
        FieldFormatter? Formatter { get; set; }

        String GetCode();

        IFieldDefinition Clone();
    }
}