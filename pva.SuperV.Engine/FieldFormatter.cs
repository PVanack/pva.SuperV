using pva.SuperV.Engine.Exceptions;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Base class for <see cref="Field{T}"/> formatting. If adding new formatters, they need to be added as JsonDerivedType annotation.
    /// </summary>
    [JsonDerivedType(typeof(EnumFormatter), typeDiscriminator: "Enum")]
    public abstract class FieldFormatter
    {
        private readonly string? _name;

        public string? Name
        {
            get => _name;
            init
            {
                IdentifierValidation.ValidateIdentifier("field value formatter", value);
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the allowed types.
        /// </summary>
        /// <value>
        /// The allowed types.
        /// </value>
        [JsonIgnore]
        public HashSet<Type> AllowedTypes { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldFormatter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="allowedTypes">The allowed types.</param>
        protected FieldFormatter(string name, HashSet<Type> allowedTypes)
        {
            Name = name;
            AllowedTypes = allowedTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldFormatter"/> class.
        /// </summary>
        /// <param name="allowedTypes">The allowed types.</param>
        protected FieldFormatter(HashSet<Type> allowedTypes)
        {
            AllowedTypes = allowedTypes;
        }

        /// <summary>
        /// Validates that type is part of the allowed types of formatter.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <exception cref="pva.SuperV.Engine.Exceptions.InvalidTypeForFormatterException"></exception>
        internal void ValidateAllowedType(Type fieldType)
        {
            if (!AllowedTypes.Contains(fieldType))
            {
                throw new InvalidTypeForFormatterException(fieldType, Name!);
            }
        }

        /// <summary>
        /// Converts a value to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>String representing the value.</returns>
        public abstract string? ConvertToString(dynamic? value);

        public abstract void ConvertFromString(IField field, string? stringValue, DateTime? timestamp, QualityLevel? quality);
    }
}