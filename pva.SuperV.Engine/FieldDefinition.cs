using pva.SuperV.Engine.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Definition of a field used in a <see cref="Class"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="pva.SuperV.Engine.IFieldDefinition" />
    public partial class FieldDefinition<T> : IFieldDefinition
    {
        /// <summary>
        /// Regex used to validate the field name.
        /// </summary>
        [GeneratedRegex(Constants.IdentifierNamePattern)]
        private static partial Regex FieldNameRegex();

        /// <summary>
        /// The name of the field. Uses <see cref="Name"/> to access value.
        /// </summary>
        private string? _name;

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string? Name
        {
            get => _name;
            set
            {
                ValidateName(value!);
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the formatter associated with field.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        public FieldFormatter? Formatter { get; set; }

        /// <summary>
        /// Gets or sets the default value for fields in instances.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public T? DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the value post change processings.
        /// </summary>
        /// <value>
        /// The value post change processings.
        /// </value>
        public List<IFieldValueProcessing> ValuePostChangeProcessings { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinition{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public FieldDefinition(string name): this(name, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinition{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="defaultValue">The default value for fields.</param>
        public FieldDefinition(string name, T? defaultValue)
        {
            this.Name = name;
            this.DefaultValue = defaultValue ?? default;
            this.Type = typeof(T);
        }

        /// <summary>
        /// Gets the C# code to create field of an <see cref="Class"/>.
        /// </summary>
        /// <returns>C# code for field.</returns>
        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"public Field<{typeof(T)}> {Name} {{ get; set; }} = new({GetStringifiedValue(DefaultValue)});");
            return codeBuilder.ToString();
        }

        private static string? GetStringifiedValue(T? defaultValue)
        {
            if (typeof(T).Equals(typeof(string))) {
                return $"\"{defaultValue}\"";
            }
            return defaultValue!.ToString();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        IFieldDefinition IFieldDefinition.Clone()
        {
            FieldDefinition<T> fieldDefinition = new(this.Name!, this.DefaultValue)
            {
                Formatter = this.Formatter
            };
            return fieldDefinition;
        }

        /// <summary>
        /// Validates the name of the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="pva.SuperV.Engine.Exceptions.InvalidFieldNameException"></exception>
        private static void ValidateName(string name)
        {
            if (!FieldNameRegex().IsMatch(name))
            {
                throw new InvalidFieldNameException(name, Constants.IdentifierNamePattern);
            }
        }
    }
}