using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InstanceAlreadyExistException : Exception
    {
        public InstanceAlreadyExistException(string? instanceName) : base($"Instance {instanceName} already exists")
        {
        }

        [ExcludeFromCodeCoverage]
        public InstanceAlreadyExistException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public InstanceAlreadyExistException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}