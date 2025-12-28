using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    [Serializable]
    internal class MixedHistoryProcessingException : SuperVException
    {
        public MixedHistoryProcessingException(string? fieldName) : base($"Field {fieldName} is used in another history processing")
        {
        }

        [ExcludeFromCodeCoverage]
        public MixedHistoryProcessingException()
        {
        }

        [ExcludeFromCodeCoverage]
        public MixedHistoryProcessingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}