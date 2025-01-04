using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an error occurs while creating an instance.</summary>
    [ExcludeFromCodeCoverage]
    public class InstanceCreationException : Exception
    {
        public InstanceCreationException(string instanceName, string className, Exception innerException) :
            base($"Error creating instance {instanceName} with class {className}", innerException)
        {
        }

        public InstanceCreationException() : base()
        {
        }

        public InstanceCreationException(string? message) : base(message)
        {
        }

        public InstanceCreationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}