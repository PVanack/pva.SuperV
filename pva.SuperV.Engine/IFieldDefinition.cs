using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.JsonConverters;
using pva.SuperV.Engine.Processing;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Field definition base interface to store <see cref="FieldDefinition{T}"/> in list/dictionnaries.
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
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the field formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        FieldFormatter? Formatter { get; set; }

        /// <summary>
        /// Gets or sets the value post change processings.
        /// </summary>
        /// <value>
        /// The value post change processings.
        /// </value>
        List<IFieldValueProcessing> ValuePostChangeProcessings { get; set; }

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