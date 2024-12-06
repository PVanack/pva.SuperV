namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InvalidProjectNameException : Exception
    {
        public InvalidProjectNameException()
        {
        }

        public InvalidProjectNameException(string? message) : base(message)
        {
        }

        public InvalidProjectNameException(string projectName, string projectNamePattern) : base($"Invalid project name {projectName}. Should comply to {projectNamePattern}")
        {
        }

        public InvalidProjectNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}