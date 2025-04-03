using pva.SuperV.Engine.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Api.Exceptions
{
    /// <summary>Exception thrown when updating an entity and changing a not changeable property.</summary>
    public class EntityPropertyNotChangeableException : SuperVException
    {
        public EntityPropertyNotChangeableException(string entityType, string propertyName)
            : base($"Property {propertyName} of {entityType} is not changeable")
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityPropertyNotChangeableException()
        {
        }

        [ExcludeFromCodeCoverage]
        public EntityPropertyNotChangeableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}