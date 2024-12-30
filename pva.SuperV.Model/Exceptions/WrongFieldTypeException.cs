using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class WrongFieldTypeException : Exception
    {
        public WrongFieldTypeException(string fieldName, Type requestedType, Type actualType) : base($"Wrong field type requested for {fieldName}: actual is {actualType} requested is {requestedType}")
        {
        }

        [ExcludeFromCodeCoverage]
        public WrongFieldTypeException()
        {
        }

        [ExcludeFromCodeCoverage]
        public WrongFieldTypeException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public WrongFieldTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}