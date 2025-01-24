namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unknown field is referenced.</summary>
    [Serializable]
    public class SuperVException : Exception
    {
        public SuperVException() : base()
        {
        }

        public SuperVException(string? message) : base(message)
        {
        }

        public SuperVException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}