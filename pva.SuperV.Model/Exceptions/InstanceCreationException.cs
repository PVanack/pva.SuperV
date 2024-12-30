using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
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