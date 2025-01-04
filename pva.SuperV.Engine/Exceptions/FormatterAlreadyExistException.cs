using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when a field formatter with same name already exists in a project.</summary>
    [Serializable]
    public class FormatterAlreadyExistException : Exception
    {
        public FormatterAlreadyExistException(string? formatterName) : base($"Field formatter {formatterName} already exists")
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