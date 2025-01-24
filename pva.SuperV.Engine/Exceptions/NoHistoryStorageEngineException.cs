using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when no history storage engine is defined.</summary>
    [Serializable]
    public class NoHistoryStorageEngineException : SuperVException
    {
        public NoHistoryStorageEngineException(string? projectName)
            : base($"Project {projectName} doesn't have a history storage engine defined")
        {
        }

        [ExcludeFromCodeCoverage]
        public NoHistoryStorageEngineException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public NoHistoryStorageEngineException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}