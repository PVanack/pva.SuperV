using System.Text;

namespace pva.SuperV.Model
{
    public class FieldDefinition<T>(String name, T? defaultValue): IFieldDefinition
    {
        public String Name { get; set; } = name;
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
