using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when a history respoitory with same name already exists in a project.</summary>
    [Serializable]
    public class HistoryRepositoryAlreadyExistException : Exception
    {
        public HistoryRepositoryAlreadyExistException(string? historyRepositoryName) : base($"History repository {historyRepositoryName} already exists")
        {
        }

        [ExcludeFromCodeCoverage]
        public HistoryRepositoryAlreadyExistException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public HistoryRepositoryAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}