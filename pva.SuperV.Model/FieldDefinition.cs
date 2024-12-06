using pva.SuperV.Model.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace pva.SuperV.Model
{
    public partial class FieldDefinition<T>(String name, T? defaultValue) : IFieldDefinition
    {
        private const string FieldNamePattern = "^([A-Z]|[a-z]|[0-9])*$";

        [GeneratedRegex(FieldNamePattern)]
        private static partial Regex FieldNameRegex();

        private string _name = name;

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
            if (!FieldNameRegex().IsMatch(value))
            {
                throw new InvalidFieldNameException(value, FieldNamePattern);
            }
        }

        public T? DefaultValue { get; set; } = defaultValue ?? default;

        public String GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"public Field<{typeof(T)}> {Name} {{ get; set; }} = new({DefaultValue});");
            return codeBuilder.ToString();
        }

        IFieldDefinition IFieldDefinition.Clone()
        {
            return new FieldDefinition<T>(this.Name, this.DefaultValue);
        }
    }
}
