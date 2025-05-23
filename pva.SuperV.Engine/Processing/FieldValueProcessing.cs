﻿namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Base class for value processing of a <see cref="Field{T}"/> trigerring field
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FieldValueProcessing<T> : IFieldValueProcessing
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the constructor arguments.
        /// </summary>
        /// <value>
        /// The constructor arguments.
        /// </value>
        public List<object> CtorArguments { get; set; } = [];

        /// <summary>
        /// Gets the field definition which triggers the processing.
        /// </summary>
        /// <value>
        /// The trigerring field definition.
        /// </value>
        public IFieldDefinition? TrigerringFieldDefinition { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldValueProcessing{T}"/> class. Ctor used for deserialization.
        /// </summary>
        protected FieldValueProcessing()
        {
        }

        protected FieldValueProcessing(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Builds the field value processing from the <see cref="CtorArguments" /> after deserialization.
        /// </summary>
        /// <param name="clazz">The clazz.</param>
        public abstract void BuildAfterDeserialization(Project project, Class clazz);

        /// <summary>
        /// Gets a ctor argument.
        /// </summary>
        /// <typeparam name="T1">The type of the argument.</typeparam>
        /// <param name="index">The index of argument.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">index</exception>
        public T1 GetCtorArgument<T1>(int index)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, CtorArguments.Count);
            return (T1)CtorArguments[index];
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

        public abstract bool IsFieldUsed(string fieldName);

        /// <summary>
        /// Gets a field definition from a class.
        /// </summary>
        /// <param name="clazz">The class from which to get field definition.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns><see cref="IFieldDefinition"/></returns>
        protected static IFieldDefinition? GetFieldDefinition(Class clazz, string? fieldName)
        {
            return string.IsNullOrEmpty(fieldName)
                ? null
                : clazz.GetField(fieldName);
        }

        /// <summary>
        /// Gets a field definition from a class.
        /// </summary>
        /// <typeparam name="T1">The type of the field definition.</typeparam>
        /// <param name="clazz">The class from which to get field definition.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns><see cref="FieldDefinition{T}"/></returns>
        protected static FieldDefinition<T1>? GetFieldDefinition<T1>(Class clazz, string? fieldName)
        {
            return string.IsNullOrEmpty(fieldName)
                ? null
                : clazz.GetField<T1>(fieldName);
        }

        /// <summary>
        /// Converts a field definition to a specific type.
        /// </summary>
        /// <typeparam name="T1">The type of the field definition.</typeparam>
        /// <param name="fieldDefinition">The field definition to convert.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns><see cref="FieldDefinition{T}"/></returns>
        protected static FieldDefinition<T1>? ConvertFieldDefinition<T1>(IFieldDefinition fieldDefinition)
        {
            return fieldDefinition as FieldDefinition<T1>;
        }

        /// <summary>
        /// Gets a field from an instance.
        /// </summary>
        /// <param name="instance">The instance from which to get the field.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns><see cref="IField"/></returns>
        protected static IField? GetInstanceField(IInstance instance, string? fieldName)
        {
            return string.IsNullOrEmpty(fieldName)
                ? null
                : instance.GetField(fieldName);
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
            return string.IsNullOrEmpty(fieldName)
                ? null
                : instance.GetField<T1>(fieldName);
        }
    }
}
