using pva.SuperV.Model.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace pva.SuperV.Model
{
    public partial class Class(String className)
    {
        private const string ClassNamePattern = "^([A-Z]|[a-z]|[0-9])*$";

        [GeneratedRegex(ClassNamePattern)]
        private static partial Regex ClassNameRegex();

        private string _name = className;

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

        private static void ValidateName(string value)
        {
            if (!ClassNameRegex().IsMatch(value))
            {
                throw new InvalidClassNameException(value, ClassNamePattern);
            }
        }

        /// <summary>Gets the fields defining the class.</summary>
        public Dictionary<String, IFieldDefinition> FieldDefinitions { get; set; } = new Dictionary<String, IFieldDefinition>(StringComparer.OrdinalIgnoreCase);

        public FieldDefinition<T> AddField<T>(FieldDefinition<T> field)
        {
            if (FieldDefinitions.ContainsKey(field.Name))
            {
                throw new FieldAlreadyExistException(field.Name);
            }

            FieldDefinitions.Add(field.Name, field);
            return field;
        }

        public void RemoveField(string fieldName)
        {
            FieldDefinitions.Remove(fieldName);
        }

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"public class {Name} : Instance {{");
            foreach (var item in FieldDefinitions)
            {
                codeBuilder.AppendLine(item.Value.GetCode());
            }
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        internal Class Clone()
        {
            var clazz = new Class(this.Name)
            {
                FieldDefinitions = new(this.FieldDefinitions.Count)
            };
            foreach (var item in FieldDefinitions)
            {
                clazz.FieldDefinitions.Add(item.Key, item.Value.Clone());
            }
            return clazz;
        }
    }
}
