using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    [JsonConverter(typeof(FieldDefinitionJsonConverter))]
    public interface IFieldDefinition
    {
        Type Type { get; set; }
        string Name { get; set; }

        String GetCode();

        IFieldDefinition Clone();
    }
}