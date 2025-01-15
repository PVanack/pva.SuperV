using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using System.Text;
using System.Text.RegularExpressions;

namespace pva.SuperV.Engine
{
    /// <summary>Dynamic class of a <see cref="Project"/>. It contains dynamic fields on which processing can be defined.</summary>
    public partial class Class
    {
        /// <summary>Regex for validating class name.</summary>
        [GeneratedRegex(Constants.IdentifierNamePattern)]
        private static partial Regex ClassNameRegex();

        /// <summary>
        /// Name of class. Access done through <see cref="Name"./>
        /// </summary>
        private string? _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Class"/> class. Used by JSON deserialization.
        /// </summary>
        public Class()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Class"/> class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public Class(string className)
        {
            this.Name = className;
        }

        /// <summary>Gets or sets the name of the class.</summary>
        public string? Name
        {
            get { return _name; }
            set
            {
                ValidateName(value!);
                _name = value;
            }
        }

        /// <summary>Gets the fields defining the class.</summary>
        public Dictionary<string, IFieldDefinition> FieldDefinitions { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Validates the name of the class.
        /// </summary>
        /// <param name="name">The name to be validated.</param>
        /// <exception cref="pva.SuperV.Engine.Exceptions.InvalidClassNameException"></exception>
        private static void ValidateName(string name)
        {
            if (!ClassNameRegex().IsMatch(name))
            {
                throw new InvalidClassNameException(name, Constants.IdentifierNamePattern);
            }
        }

        /// <summary>
        /// Adds a field definition to the class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <returns>The field once it has been added.</returns>
        internal FieldDefinition<T> AddField<T>(FieldDefinition<T> field)
        {
            return AddField<T>(field, null);
        }

        /// <summary>
        /// Adds a field with a field formatter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The <see cref="FieldDefinition{T}" to be added.</param>
        /// <param name="formatter">The formatter to be used when using ToString()."/>.</param>
        /// <returns></returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.FieldAlreadyExistException"></exception>
        internal FieldDefinition<T> AddField<T>(FieldDefinition<T> field, FieldFormatter? formatter)
        {
            if (FieldDefinitions.ContainsKey(field.Name!))
            {
                throw new FieldAlreadyExistException(field.Name);
            }
            field.Formatter = formatter;
            FieldDefinitions.Add(field.Name!, field);
            return field;
        }

        internal void AddFieldChangePostProcessing<T>(string fieldName, FieldValueProcessing<T> fieldValueProcessing)
        {
            FieldDefinition<T>? fieldDefinition = GetField<T>(fieldName);
            fieldDefinition!.ValuePostChangeProcessings.Add(fieldValueProcessing!);
        }

        /// <summary>
        /// Gets a field of a specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field to be retrieved.</param>
        /// <returns>The field</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.UnknownFieldException"></exception>
        public FieldDefinition<T>? GetField<T>(string fieldName)
        {
            if (FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldDefinition))
            {
                if (fieldDefinition.Type == typeof(T))
                {
                    return fieldDefinition as FieldDefinition<T>;
                }
                throw new WrongFieldTypeException(fieldName, typeof(T), fieldDefinition.Type);
            }
            throw new UnknownFieldException(fieldName);
        }

        /// <summary>
        /// Removes a field.
        /// </summary>
        /// <param name="fieldName">Name of the field to be removed.</param>
        internal void RemoveField(string fieldName)
        {
            FieldDefinitions.Remove(fieldName);
        }

        /// <summary>
        /// Gets the C# code for the class.
        /// </summary>
        /// <returns>C# code of class</returns>
        internal string GetCode()
        {
            StringBuilder codeBuilder = new();
            StringBuilder ctorBuilder = new($"public {Name}() {{");
            codeBuilder.AppendLine($"public class {Name} : Instance {{");
            FieldDefinitions
                .ForEach((k, v) =>
                {
                    codeBuilder.AppendLine(v.GetCode());
                    ctorBuilder.AppendFormat("Fields.Add(\"{0}\", {0});{0}.Instance = this;", k).AppendLine();
                });
            ctorBuilder.AppendLine("}");
            codeBuilder.AppendLine(ctorBuilder.ToString());
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A new <see cref="Class"/> clone of class instance.</returns>
        internal Class Clone()
        {
            var clazz = new Class(this.Name!);
            FieldDefinitions
                .ForEach((k, v) => clazz.FieldDefinitions.Add(k, v.Clone()));
            return clazz;
        }
    }
}