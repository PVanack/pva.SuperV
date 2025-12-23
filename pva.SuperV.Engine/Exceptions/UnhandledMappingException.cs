using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    public class UnhandledMappingException : SuperVException
    {
        public UnhandledMappingException(string mapperName, string? entityType)
            : base($"Unhandled mapping for {mapperName} {entityType}")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnhandledMappingException()
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
