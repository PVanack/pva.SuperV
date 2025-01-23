using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when name of a history repository contains invalid character(s).</summary>
    [Serializable]
    public class InvalidHistoryRepositoryNameException : Exception
    {
        public InvalidHistoryRepositoryNameException(string historyRepositoryName, string historyRepositoryNamePattern) : base($"Invalid history repository name {historyRepositoryName}. Should comply to {historyRepositoryNamePattern}")
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidHistoryRepositoryNameException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidHistoryRepositoryNameException()
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidHistoryRepositoryNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}