using pva.SuperV.Model.Exceptions;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace pva.SuperV.Model
{
    /// <summary>
    /// Base class for <see cref="Field{T}"/> formatting. If adding new formatters, they need to be added as JsonDerivedType annotation.
    /// </summary>
    [JsonDerivedType(typeof(EnumFormatter), typeDiscriminator: "Enum")]
    public abstract partial class FieldFormatter
    {
        /// <summary>
        /// Regex for validating the field formatter's name.
        /// </summary>
        [GeneratedRegex(Constants.IdentifierNamePattern)]
        private static partial Regex FieldFormatterNameRegex();

        private string? _name;

        public string? Name
        {
            get => _name;
            init
            {
                ValidateName(value!);
                _name = value;
            }
        }

        [JsonIgnore]
        public HashSet<Type> AllowedTypes { get; set; }

        protected FieldFormatter(string name, HashSet<Type> allowedTypes)
        {
            this.Name = name;
            this.AllowedTypes = allowedTypes;
        }

        protected FieldFormatter(HashSet<Type> allowedTypes)
        {
            AllowedTypes = allowedTypes;
        }

        private static void ValidateName(string name)
        {
            if (!FieldFormatterNameRegex().IsMatch(name))
            {
                throw new InvalidFormatterNameException(name, Constants.IdentifierNamePattern);
            }
        }

        internal void ValidateAllowedType(Type fieldType)
        {
            if (!AllowedTypes.Contains(fieldType))
            {
                throw new InvalidTypeForFormatterException(fieldType, Name!);
            }
        }

        public abstract string? ConvertToString(dynamic? value);
    }
}