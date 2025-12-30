using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an entity with same name already exists in project.</summary>
    public class EntityAlreadyExistException : SuperVException
    {
        public EntityAlreadyExistException(string entityType, string? entityName)
            : base($"{entityType} {entityName} already exists")
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityAlreadyExistException()
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