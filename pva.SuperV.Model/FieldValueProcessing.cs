namespace pva.SuperV.Model
{
    /// <summary>
    /// Base class for value processing of a <see cref="Field{T}"/> trigerring field
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FieldValueProcessing<T>
    {
        /// <summary>
        /// Gets or sets the field definition which triggers the processing..
        /// </summary>
        /// <value>
        /// The trigerring field definition.
        /// </value>
        public FieldDefinition<T>? TrigerringFieldDefinition { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string? Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueProcessing{T}"/> class. Ctor used for deserialization.
        /// </summary>
        protected FieldValueProcessing()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueProcessing{T}"/> class.
        /// </summary>
        /// <param name="name">The name of processing.</param>
        protected FieldValueProcessing(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Processes the value change.
        /// </summary>
        /// <param name="instance">Instance on which the triggering field changed.</param>
        /// <param name="changedField">The <see cref="Field{T}"/> which changed.</param>
        /// <param name="valueChanged">If <c>true</c>, indicates that the trigerring field value changed.</param>
        /// <param name="previousValue">The previous value of field.</param>
        /// <param name="currentValue">The current value of field.</param>
        public abstract void ProcessValue(IInstance instance, Field<T> changedField, bool valueChanged, T previousValue, T currentValue);

        /// <summary>
        /// Gets a field definition from a class.
        /// </summary>
        /// <typeparam name="T1">The type of the field definition.</typeparam>
        /// <param name="clazz">The class from which to get field definition.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns><see cref="FieldDefinition{T}"/></returns>
        protected static FieldDefinition<T1>? GetFieldDefinition<T1>(Class clazz, string? fieldName)
        {
            return String.IsNullOrEmpty(fieldName)
                ? null
                : clazz.GetField<T1>(fieldName);
        }

        /// <summary>
        /// Gets a field from an instance.
        /// </summary>
        /// <typeparam name="T1">The type of the field.</typeparam>
        /// <param name="instance">The instance from which to get the field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns><see cref="Field{T}"/></returns>
        protected static Field<T1>? GetInstanceField<T1>(IInstance instance, string? fieldName)
        {
            return String.IsNullOrEmpty(fieldName)
                ? null
                : instance.GetField<T1>(fieldName);
        }
    }
}
