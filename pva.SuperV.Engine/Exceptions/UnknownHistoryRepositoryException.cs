using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unknown history repository is referenced.</summary>
    [Serializable]
    public class UnknownHistoryRepositoryException : Exception
    {
        public UnknownHistoryRepositoryException(string? historyRepositoryName) : base($"History repository {historyRepositoryName} doesn't exist")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownHistoryRepositoryException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownHistoryRepositoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}