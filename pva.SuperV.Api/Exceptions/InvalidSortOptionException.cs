using pva.SuperV.Engine.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace pva.SuperV.Api.Exceptions
{
    /// <summary>Exception thrown when trying to build from a non WIP project.</summary>
    public class InvalidSortOptionException : SuperVException
    {
        public InvalidSortOptionException(string sortOption, List<string> validSortOptions)
            : base($"Invalid sort option {sortOption}. Valid sort options are {validSortOptions.Select(opt => $"[-]{opt}").Aggregate((a, b) => $"{a}, {b}")}.")
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidSortOptionException()
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidSortOptionException(string message) : base(message)
        {
        }

        [ExcludeFromCodeCoverage]
        public InvalidSortOptionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}