namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InvalidFieldNameException : Exception
    {
        public InvalidFieldNameException()
        {
        }

        public InvalidFieldNameException(string? message) : base(message)
        {
        }

        public InvalidFieldNameException(string fieldName, string fieldNamePattern) : base($"Invalid field name {fieldName}. Should comply to {fieldNamePattern}")
        {
        }

        public InvalidFieldNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}