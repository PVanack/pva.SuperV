using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when no history repository can be found through a field having a <see cref="Processing.HistorizationProcessing{T}"/>.</summary>
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