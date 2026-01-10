using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    public class ScriptSyntaxErrorException :  SuperVException
    {
        public ScriptSyntaxErrorException(string message, string line, int errorPosition)
            : base($"Syntax error ({message}) at position {errorPosition} in {line}.")
        {
        }

        [ExcludeFromCodeCoverage]
        public ScriptSyntaxErrorException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public ScriptSyntaxErrorException()
        {
        }

        [ExcludeFromCodeCoverage]
        public ScriptSyntaxErrorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
