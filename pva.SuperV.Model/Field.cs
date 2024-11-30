using System.Text;

namespace pva.SuperV.Model
{
    public class Field<T>(String name, T? value)
    {
        public String Name { get; set; } = name;
        public T? Value { get; set; } = value ?? default;

        public String GetCode()
        {
            StringBuilder codeBuilder = new ();
            codeBuilder.AppendLine($"public {typeof(T)} {Name} {{ get; set; }} = {Value};");
            return codeBuilder.ToString();
        }
    }
}
