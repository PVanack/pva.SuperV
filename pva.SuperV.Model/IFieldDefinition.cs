using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    /// <summary>
    /// Field definition base class to store those if list/dictionnaries.
    /// </summary>
    [JsonConverter(typeof(FieldDefinitionJsonConverter))]
    public interface IFieldDefinition
    {
        /// <summary>
        /// Gets or sets the type of the field definition.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type { get; set; }
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string? Name { get; set; }
        /// <summary>
        /// Gets or sets the field formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        FieldFormatter? Formatter { get; set; }

        /// <summary>
        /// Gets the C# code representation of the field in class.
        /// </summary>
        /// <returns>C# code</returns>
        String GetCode();

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        IFieldDefinition Clone();
    }
}