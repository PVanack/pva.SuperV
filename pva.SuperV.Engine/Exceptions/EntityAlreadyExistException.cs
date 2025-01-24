using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when a field with same name already exists in a class.</summary>
    [Serializable]
    public class EntityAlreadyExistException : SuperVException
    {
        public EntityAlreadyExistException(string entityType, string? entityName)
            : base($"{entityType} {entityName} already exists")
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityAlreadyExistException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityAlreadyExistException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}