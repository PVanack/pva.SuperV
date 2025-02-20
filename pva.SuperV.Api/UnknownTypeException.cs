using pva.SuperV.Engine.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Api
{
    /// <summary>Exception thrown when trying to build from a non WIP project.</summary>
    [Serializable]
    public class UnknownTypeException : SuperVException
    {
        public UnknownTypeException(string entityType, string? typeName)
            : base($"Class {typeName} for {entityType} doesn't exist")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownTypeException(string message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownTypeException()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}