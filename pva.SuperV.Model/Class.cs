using pva.Helpers;
using pva.SuperV.Model.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace pva.SuperV.Model
{
    public partial class Class : IDisposable
    {
        private const string ClassNamePattern = "^([A-Z]|[a-z]|[0-9])*$";

        [GeneratedRegex(ClassNamePattern)]
        private static partial Regex ClassNameRegex();

        private string _name;

        public Class(String className)
        {
            this.Name = className;
        }
        /// <summary>Gets or sets the name of the class.</summary>
        public String Name
        {
            get { return _name; }
            set
            {
                ValidateName(value);
                _name = value;
            }
        }

        /// <summary>Gets the fields defining the class.</summary>
        public Dictionary<String, IFieldDefinition> FieldDefinitions { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        private static void ValidateName(string name)
        {
            if (!ClassNameRegex().IsMatch(name))
            {
                throw new InvalidClassNameException(name, ClassNamePattern);
            }
        }

        public FieldDefinition<T> AddField<T>(FieldDefinition<T> field)
        {
            if (FieldDefinitions.ContainsKey(field.Name))
            {
                throw new FieldAlreadyExistException(field.Name);
            }

            FieldDefinitions.Add(field.Name, field);
            return field;
        }

        public FieldDefinition<T> GetField<T>(string fieldName)
        {
            if (FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition fieldDefinition))
            {
                return fieldDefinition as FieldDefinition<T>;
            }
            throw new UnknownFieldException(fieldName);
        }

        public void RemoveField(string fieldName)
        {
            FieldDefinitions.Remove(fieldName);
        }

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"public class {Name} : Instance {{");
            FieldDefinitions
                .ForEach((_, v) => codeBuilder.AppendLine(v.GetCode()));
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        internal Class Clone()
        {
            var clazz = new Class(this.Name)
            {
                FieldDefinitions = new(this.FieldDefinitions.Count)
            };
            FieldDefinitions
                .ForEach((k, v) => clazz.FieldDefinitions.Add(k, v.Clone()));
            return clazz;
        }

        public void Dispose()
        {
        }
    }
}
