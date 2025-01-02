using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class FormatterAlreadyExistException : Exception
    {
        public FormatterAlreadyExistException(string? enumName) : base($"Enum {enumName} already exists")
        {
        }

        [ExcludeFromCodeCoverage]
        public FormatterAlreadyExistException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public FormatterAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}