using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.Model
{
    public class Instance : IInstance
    {
        public Dictionary<string, IField> Fields { get; set; } = [];
        public string Name { get; set; }

        public Class Class { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Fields.Clear();
        }

        public Field<T> GetField<T>(string fieldName)
        {
            IField field = GetField(fieldName);
            if (field.Type != (typeof(T)))
            {
                throw new WrongFieldTypeException(fieldName, field.Type, typeof(T));
            }
            return field as Field<T>;
        }

        public IField GetField(string fieldName)
        {
            if (!Class.FieldDefinitions.ContainsKey(fieldName))
            {
                throw new UnknownFieldException(fieldName, Class.Name);
            }
            return Fields[fieldName];
        }
    }
}