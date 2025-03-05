﻿using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when history engine connection string contains an unknown type.</summary>
    [Serializable]
    public class UnknownHistoryStorageEngineException : SuperVException
    {
        [ExcludeFromCodeCoverage]
        public UnknownHistoryStorageEngineException(string? connectionString)
            : base($"Unknown history storage engine connection string: {connectionString}")
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownHistoryStorageEngineException() : base()
        {
        }

        [ExcludeFromCodeCoverage]
        public UnknownHistoryStorageEngineException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}