using pva.SuperV.Engine.Exceptions;
using System.Text.RegularExpressions;

namespace pva.SuperV.Engine
{
    internal static partial class IdentifierValidation
    {
        /// <summary>Regex for validating identifier name.</summary>
        [GeneratedRegex(Constants.IdentifierNamePattern)]
        private static partial Regex IdentifierNameRegex();

        public static string ValidateIdentifier(string entityType, string? identifier)
        {
            if (String.IsNullOrEmpty(identifier) || !IdentifierNameRegex().IsMatch(identifier))
            {
                throw new InvalidIdentifierNameException(entityType, identifier, Constants.IdentifierNamePattern);
            }
            return identifier;
        }
    }
}
