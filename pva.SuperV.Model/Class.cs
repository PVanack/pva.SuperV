using pva.SuperV.Model.Exceptions;
using System.Text;

namespace pva.SuperV.Model
{
    public class Class(String className)
    {
        //TODO Validate class name with regex
        /// <summary>Gets or sets the name of the class.</summary>
        public String Name { get; set; } = className;
        /// <summary>Gets the fields defining the class.</summary>
        public Dictionary<String, dynamic> Fields { get; init; } = new Dictionary<String, dynamic>(StringComparer.OrdinalIgnoreCase);

        public Field<T> AddField<T>(Field<T> field)
        {
            if (Fields.ContainsKey(field.Name))
            {
                throw new FieldAlreadyExistException(field.Name);
            }

            Fields.Add(field.Name, field);
            return field;
        }

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"public class {Name} {{");
            codeBuilder.AppendLine("public System.String Name { get; set; }");
            foreach (var item in Fields)
            {
                codeBuilder.AppendLine(item.Value.GetCode());
            }
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }
    }
}
