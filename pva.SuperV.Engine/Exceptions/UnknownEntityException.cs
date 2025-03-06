using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unknown entity is referenced.</summary>
    public class UnknownEntityException : SuperVException
    {
        public UnknownEntityException(string entityType, string? entityName)
            : base($"{entityType} {entityName} doesn't exist")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownEntityException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownEntityException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownEntityException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}