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

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"public class {Name} {{");
            codeBuilder.AppendLine("public System.String Name { get; set; }");
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
