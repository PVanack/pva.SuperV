using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class UnknownInstanceException : Exception
    {
        public UnknownInstanceException(string? instanceName) : base($"Instance {instanceName} doesn't exist")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownInstanceException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownInstanceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}