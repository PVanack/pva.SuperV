using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when name of a contains invalid character(s).</summary>
    [Serializable]
    public class InvalidIdentifierNameException : SuperVException
    {
        public InvalidIdentifierNameException(string entityType, string? identifier, string identifierNamePattern)
            : base($"Invalid {entityType} name {identifier}. Should comply to {identifierNamePattern}")
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidIdentifierNameException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidIdentifierNameException()
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidIdentifierNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}