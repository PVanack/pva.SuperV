﻿using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Engine.Exceptions
{
    /// <summary>Exception thrown when a TDengine error occurs.</summary>
    public class TdEngineException : SuperVException
    {
        [ExcludeFromCodeCoverage]
        public TdEngineException(string operation, Exception innerException)
            : base($"TDengine {operation} failure: {innerException.Message}", innerException)
        {
        }

        [ExcludeFromCodeCoverage]
        public TdEngineException()
        {
        }

        [ExcludeFromCodeCoverage]
        public TdEngineException(string? message) : base(message)
        {
        }
    }
}