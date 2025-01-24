using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when type requested doesn't match actual field type.</summary>
    [Serializable]
    public class TdEngineException : SuperVException
    {
        public TdEngineException(string operation, Exception innerException)
            : base($"TDengine {operation} failure: {innerException.Message}")
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