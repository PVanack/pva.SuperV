using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    [JsonConverter(typeof(InstanceJsonConverter))]
    public interface IInstance : IDisposable
    {
        public String Name { get; set; }
        public Class Class { get; set; }
        public Dictionary<string, IField> Fields { get; set; }
    }
}