using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.Model
{
    /// <summary>
    /// Generated instance.
    /// </summary>
    /// <seealso cref="pva.SuperV.Model.IInstance" />
    public class Instance : IInstance
    {
        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the class of the instance.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public Class Class { get; set; } = null!;

        /// <summary>
        /// Gets or sets the fields contained in instance.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public Dictionary<string, IField> Fields { get; set; } = [];

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            Fields.Clear();
        }

        /// <summary>
        /// Gets a field of the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The field.</returns>
        /// <exception cref="pva.SuperV.Model.Exceptions.WrongFieldTypeException"></exception>
        public Field<T>? GetField<T>(string fieldName)
        {
            IField field = GetField(fieldName);
            if (field.Type != (typeof(T)))
            {
                throw new WrongFieldTypeException(fieldName, field.Type, typeof(T));
            }
            return field as Field<T>;
        }

        /// <summary>
        /// Gets a field of the instance.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The field.</returns>
        /// <exception cref="pva.SuperV.Model.Exceptions.UnknownFieldException"></exception>
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