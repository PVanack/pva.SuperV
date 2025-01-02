using pva.Helpers.Extensions;
using pva.SuperV.Model.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace pva.SuperV.Model
{
    public partial class Class
    {
        [GeneratedRegex(Constants.IdentifierNamePattern)]
        private static partial Regex ClassNameRegex();

        private string? _name;

        public Class()
        {
        }

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

        private static void ValidateName(string name)
        {
            if (!ClassNameRegex().IsMatch(name))
            {
                throw new InvalidClassNameException(name, Constants.IdentifierNamePattern);
            }
        }

        internal FieldDefinition<T> AddField<T>(FieldDefinition<T> field)
        {
            return AddField<T>(field, null);
        }

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

        public FieldDefinition<T>? GetField<T>(string fieldName)
        {
            if (FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldDefinition))
            {
                return fieldDefinition as FieldDefinition<T>;
            }
            throw new UnknownFieldException(fieldName);
        }

        internal void RemoveField(string fieldName)
        {
            FieldDefinitions.Remove(fieldName);
        }

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            StringBuilder ctorBuilder = new($"public {Name}() {{");
            codeBuilder.AppendLine($"public class {Name} : Instance {{");
            FieldDefinitions
                .ForEach((k, v) =>
                {
                    codeBuilder.AppendLine(v.GetCode());
                    ctorBuilder.AppendFormat("Fields.Add(\"{0}\", {0});", k).AppendLine();
                });
            ctorBuilder.AppendLine("}");
            codeBuilder.AppendLine(ctorBuilder.ToString());
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        internal Class Clone()
        {
            var clazz = new Class(this.Name!)
            {
                FieldDefinitions = new(this.FieldDefinitions.Count)
            };
            FieldDefinitions
                .ForEach((k, v) => clazz.FieldDefinitions.Add(k, v.Clone()));
            return clazz;
        }
    }
}