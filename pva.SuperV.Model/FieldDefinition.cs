using pva.SuperV.Model.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace pva.SuperV.Model
{
    public partial class FieldDefinition<T> : IFieldDefinition
    {
        private const string FieldNamePattern = "^([A-Z]|[a-z]|[0-9]|_)*$";

        [GeneratedRegex(FieldNamePattern)]
        private static partial Regex FieldNameRegex();

        private string? _name;

        public string? Name
        {
            get => _name;
            set
            {
                ValidateName(value!);
                _name = value;
            }
        }

        public Type Type { get; set; }

        private static void ValidateName(string name)
        {
            if (!FieldNameRegex().IsMatch(name))
            {
                throw new InvalidFieldNameException(name, FieldNamePattern);
            }
        }

        public T? DefaultValue { get; set; }

        public FieldDefinition(string name, T? defaultValue)
        {
            this.Name = name;
            this.DefaultValue = defaultValue ?? default;
            this.Type = typeof(T);
        }

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"public Field<{typeof(T)}> {Name} {{ get; set; }} = new({DefaultValue});");
            return codeBuilder.ToString();
        }

        IFieldDefinition IFieldDefinition.Clone()
        {
            return new FieldDefinition<T>(this.Name!, this.DefaultValue);
        }
    }
}