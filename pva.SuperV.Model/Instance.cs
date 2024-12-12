using pva.Helpers;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.Model
{
    public class Instance : IInstance
    {
        public Dictionary<string, IField> Fields { get; } = new();
        public string Name { get; set; }

        public Class Class { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Fields.Values
                .ForEach(field => field.Dispose());
            Fields.Clear();
        }
        public Field<T> GetField<T>(string fieldName)
        {
            if (!Class.FieldDefinitions.ContainsKey(fieldName))
            {
                throw new UnknownFieldException(fieldName, Class.Name);
            }
            IField field = Fields[fieldName];
            if (field.Type != (typeof(T)))
            {
                throw new WrongFieldTypeException(fieldName, field.Type, typeof(T));
            }
            return field as Field<T>;
        }
    }
}
