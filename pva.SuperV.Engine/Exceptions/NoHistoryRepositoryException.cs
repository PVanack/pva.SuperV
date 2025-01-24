using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unknown history repository is referenced.</summary>
    [Serializable]
    public class NoHistoryRepositoryException : SuperVException
    {
        public NoHistoryRepositoryException()
            : base("No history repository found in fields")
        {
        }

        [ExcludeFromCodeCoverage]
        public NoHistoryRepositoryException(string? message) : base(message)
        {
        }
        [ExcludeFromCodeCoverage]
        public NoHistoryRepositoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}