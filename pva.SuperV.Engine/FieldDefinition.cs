using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.Processing;
using System.Globalization;
using System.Text;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Definition of a field used in a <see cref="Class"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="pva.SuperV.Engine.IFieldDefinition" />
    public class FieldDefinition<T> : IFieldDefinition
    {
        /// <summary>
        /// The name of the field. Uses <see cref="Name"/> to access value.
        /// </summary>
        private string _name;

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                IdentifierValidation.ValidateIdentifier("field", value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; init; }

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
        public FieldDefinition(string name) : this(name, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinition{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="defaultValue">The default value for fields.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public FieldDefinition(string name, T? defaultValue)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            Name = name;
            DefaultValue = defaultValue ?? default;
            Type = typeof(T);
        }

        /// <summary>
        /// Gets the C# code to create field of an <see cref="Class"/>.
        /// </summary>
        /// <returns>C# code for field.</returns>
        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine(
                $"public Field<{typeof(T)}> {Name} {{ get; set; }} = new ({GetCodeValueForNew(DefaultValue)});");
            return codeBuilder.ToString();
        }

        private static string? GetCodeValueForNew(T? defaultValue)
        {
            if (typeof(T).Equals(typeof(string)))
            {
                return $"\"{defaultValue}\"";
            }
            else
            {

                return defaultValue switch
                {
                    bool boolValue => boolValue ? "true" : "false",
                    DateTime dateTimeValue => $"new {typeof(T)}({dateTimeValue.Ticks.ToString(CultureInfo.InvariantCulture)}L)",
                    TimeSpan timespanValue => $"new {typeof(T)}({timespanValue.Ticks.ToString(CultureInfo.InvariantCulture)}L)",
                    float floatValue => $"{floatValue.ToString(CultureInfo.InvariantCulture)}F",
                    double doubleValue => $"{doubleValue.ToString(CultureInfo.InvariantCulture)}",
                    _ => defaultValue!.ToString(),
                };
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        IFieldDefinition IFieldDefinition.Clone()
        {
            FieldDefinition<T> fieldDefinition = new(Name, DefaultValue)
            {
                Formatter = Formatter,
                ValuePostChangeProcessings = [.. ValuePostChangeProcessings]
            };
            return fieldDefinition;
        }

        /// <summary>
        /// Update field from another field.
        /// </summary>
        /// <param name="fieldDefinitionUpdate">Field from which to update. Only default value and formatter are copied.</param>
        /// <param name="fieldFormatter">Formatter</param>
        /// <exception cref="WrongFieldTypeException"></exception>
        public void Update(IFieldDefinition fieldDefinitionUpdate, FieldFormatter? fieldFormatter)
        {
            if (fieldDefinitionUpdate is FieldDefinition<T> typedFieldDefinitionUpdate)
            {
                DefaultValue = typedFieldDefinitionUpdate.DefaultValue;
                Formatter = fieldFormatter;
                return;
            }
            throw new WrongFieldTypeException(Name, Type, fieldDefinitionUpdate.Type);
        }
    }
}