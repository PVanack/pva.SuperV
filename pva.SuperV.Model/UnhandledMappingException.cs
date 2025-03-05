using pva.SuperV.Engine.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model
{
    public class UnhandledMappingException : SuperVException
    {
        public UnhandledMappingException(string mapperName, string? entityType)
            : base($"Unhandled mapping for {mapperName} {entityType}")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledMappingException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledMappingException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledMappingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
