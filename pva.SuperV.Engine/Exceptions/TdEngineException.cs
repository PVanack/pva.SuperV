using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when a TDengine error occurs.</summary>
    [Serializable]
    public class TdEngineException : SuperVException
    {
        public TdEngineException(string operation, Exception innerException)
            : base($"TDengine {operation} failure: {innerException.Message}", innerException)
        {
        }

        [ExcludeFromCodeCoverage]
        public TdEngineException()
        {
        }

        [ExcludeFromCodeCoverage]
        public TdEngineException(string? message) : base(message)
        {
        }
    }
}