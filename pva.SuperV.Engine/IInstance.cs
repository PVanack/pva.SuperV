using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.JsonConverters;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Used to store dynamically generated <see cref="Instance"/> in dictionnaries.
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

        /// <summary>
        /// Gets a field of the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The field.</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.WrongFieldTypeException"></exception>
        Field<T>? GetField<T>(string fieldName);

        /// <summary>
        /// Gets a field of the instance.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The field.</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.UnknownFieldException"></exception>
        IField GetField(string fieldName);
    }
}