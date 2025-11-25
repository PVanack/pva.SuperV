using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.Processing;
using System.Text;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>Dynamic class of a <see cref="Project"/>. It contains dynamic fields on which processing can be defined.</summary>
    public class Class
    {
        private const string FieldEntityType = "Field";

        /// <summary>
        /// Name of class. Access done through <see cref="Name"/>.
        /// </summary>
        private string _name = string.Empty;

        /// <summary>Gets or sets the name of the class.</summary>
        public string Name
        {
            get { return _name; }
            set
            {
                IdentifierValidation.ValidateIdentifier("class", value);
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the base class.
        /// </summary>
        /// <value>
        /// The base class.
        /// </value>
        [JsonIgnore]
        public Class? BaseClass { get; set; }

        /// <summary>
        /// Gets or sets the name of the base class.
        /// </summary>
        /// <value>
        /// The name of the base class.
        /// </value>
        public string? BaseClassName { get; set; }

        /// <summary>Gets the fields defining the class.</summary>
        public Dictionary<string, IFieldDefinition> FieldDefinitions { get; set; } = new(StringComparer.OrdinalIgnoreCase);

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
        public Class(string className) : this(className, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Class"/> class with inheritance from a base class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="baseClass">Base class.</param>
        public Class(string className, Class? baseClass)
        {
            this.Name = className;
            this.BaseClass = baseClass;
            this.BaseClassName = baseClass?.Name;
        }

        /// <summary>
        /// Adds a field definition to the class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The field once it has been added.</returns>
        public IFieldDefinition AddField(IFieldDefinition field)
        {
            return AddField(field, null);
        }

        /// <summary>
        /// Adds a field with a field formatter.
        /// </summary>
        /// <param name="field">The <see cref="FieldDefinition{T}"/> to be added.</param>
        /// <param name="formatter">The formatter to be used when using ToString()."/>.</param>
        /// <returns></returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.EntityAlreadyExistException"></exception>
        public IFieldDefinition AddField(IFieldDefinition field, FieldFormatter? formatter)
        {
            if (FieldDefinitions.ContainsKey(field.Name))
            {
                throw new EntityAlreadyExistException(FieldEntityType, field.Name);
            }
            field.Formatter = formatter;
            FieldDefinitions.Add(field.Name, field);
            return field;
        }

        public IFieldDefinition UpdateField(string fieldName, IFieldDefinition fieldDefinition, FieldFormatter? fieldFormatter)
        {
            if (FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldToUpdate))
            {
                fieldToUpdate.Update(fieldDefinition, fieldFormatter);
                return fieldToUpdate;
            }
            throw new UnknownEntityException(FieldEntityType, fieldName);
        }

        /// <summary>
        /// Gets a field.
        /// </summary>
        /// <param name="fieldName">Name of the field to be retrieved.</param>
        /// <returns>The field</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.UnknownEntityException"></exception>
        public IFieldDefinition GetField(string fieldName)
        {
            if (FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldDefinition))
            {
                return fieldDefinition;
            }
            if (BaseClass is not null)
            {
                return BaseClass.GetField(fieldName);
            }
            throw new UnknownEntityException(FieldEntityType, fieldName);
        }

        /// <summary>
        /// Gets a field of a specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field to be retrieved.</param>
        /// <returns>The field</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.WrongFieldTypeException"></exception>
        /// <exception cref="pva.SuperV.Engine.Exceptions.UnknownEntityException"></exception>
        public FieldDefinition<T>? GetField<T>(string fieldName)
        {
            IFieldDefinition fieldDefinition = GetField(fieldName);
            if (fieldDefinition.Type == typeof(T))
            {
                return fieldDefinition as FieldDefinition<T>;
            }
            throw new WrongFieldTypeException(fieldName, typeof(T), fieldDefinition.Type);
        }

        /// <summary>
        /// Removes a field.
        /// </summary>
        /// <param name="fieldName">Name of the field to be removed.</param>
        public void RemoveField(string fieldName)
        {
            VerifyFieldNotUsedInProcessings(fieldName);
            FieldDefinitions.Remove(fieldName);
        }

        internal void AddFieldChangePostProcessing(string fieldName, IFieldValueProcessing fieldValueProcessing)
        {
            IFieldDefinition? fieldDefinition = GetField(fieldName);
            fieldDefinition!.ValuePostChangeProcessings.Add(fieldValueProcessing!);
        }

        internal void UpdateFieldChangePostProcessing(string fieldName, string processingName, IFieldValueProcessing fieldProcessing)
        {
            IFieldDefinition? fieldDefinition = GetField(fieldName);
            IFieldValueProcessing? fieldValueProcessingToUpdate = fieldDefinition?.ValuePostChangeProcessings
                .FirstOrDefault(valueProcessing => valueProcessing.Name == processingName);
            if (fieldValueProcessingToUpdate != null)
            {
                fieldDefinition!.ValuePostChangeProcessings.Remove(fieldValueProcessingToUpdate);
                fieldDefinition!.ValuePostChangeProcessings.Add(fieldProcessing);
                return;
            }
            throw new UnknownEntityException("Field processing", processingName);
        }

        internal void RemoveFieldChangePostProcessing(string fieldName, string processingName)
        {
            if (FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldDefinition))
            {
                IFieldValueProcessing? processing = fieldDefinition.ValuePostChangeProcessings
                    .FirstOrDefault(fieldProcessing => fieldProcessing.Name.Equals(processingName));
                if (processing is not null)
                {
                    fieldDefinition.ValuePostChangeProcessings.Remove(processing);
                    return;
                }
                throw new UnknownEntityException("Field processing", processingName);
            }
            throw new UnknownEntityException(FieldEntityType, fieldName);
        }

        private void VerifyFieldNotUsedInProcessings(string fieldName)
        {
            List<String> fieldsUsedInProcessing = [.. FieldDefinitions.Values
                    .Where(field => !field.Name.Equals(fieldName) && field.ValuePostChangeProcessings.Any(valueProcessing => valueProcessing.IsFieldUsed(fieldName)))
                    .Select(field => field.Name)];
            if (fieldsUsedInProcessing.Count > 0)
            {
                throw new EntityInUseException(FieldEntityType, fieldName, Name, fieldsUsedInProcessing);
            }
        }

        /// <summary>
        /// Gets the C# code for the class.
        /// </summary>
        /// <returns>C# code of class</returns>
        internal string GetCode()
        {
            StringBuilder codeBuilder = new();
            StringBuilder ctorBuilder = new($"public {Name}() {{");
            string baseClass = BaseClass is null ? "Instance" : BaseClass.Name;
            codeBuilder.AppendLine($"public class {Name} : {baseClass} {{");
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
            var clazz = new Class(Name, BaseClass);
            FieldDefinitions
                .ForEach((k, v) => clazz.FieldDefinitions.Add(k, v.Clone()));
            return clazz;
        }
    }
}