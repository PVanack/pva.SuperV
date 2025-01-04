using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an instance with same name already exists in a project.</summary>
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