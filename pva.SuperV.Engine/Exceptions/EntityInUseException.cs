using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an entity is in use by another entity.</summary>
    public class EntityInUseException : SuperVException
    {
        public EntityInUseException(string entityType, string entityName, string className, List<string> usingFields) :
            base($"{entityType} {entityName} is in use by class {className}'s fields: {usingFields.Aggregate((a, b) => $"{a},{b}")}")
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityInUseException()
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityInUseException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityInUseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}