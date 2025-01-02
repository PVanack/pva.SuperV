using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    /// <summary>
    /// Base class of <see cref="Instance"/> to store dynamically generated instances in dictionnaries.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    [JsonConverter(typeof(InstanceJsonConverter))]
    public interface IInstance : IDisposable
    {
        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String Name { get; set; }
        /// <summary>
        /// Gets or sets the class of the instance.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public Class Class { get; set; }
        /// <summary>
        /// Gets or sets the fields contained in instance.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public Dictionary<string, IField> Fields { get; set; }
    }
}