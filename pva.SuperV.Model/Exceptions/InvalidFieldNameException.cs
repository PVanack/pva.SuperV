﻿using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Model.Exceptions
{
    [Serializable]
    public class InvalidFieldNameException : Exception
    {
        public InvalidFieldNameException(string fieldName, string fieldNamePattern) : base($"Invalid field name {fieldName}. Should comply to {fieldNamePattern}")
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidFieldNameException(string? message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidFieldNameException()
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidFieldNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}