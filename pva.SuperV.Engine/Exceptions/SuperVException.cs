﻿using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when an unknown field is referenced.</summary>
    [Serializable]
    public class SuperVException : Exception
    {
        public SuperVException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public SuperVException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public SuperVException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}