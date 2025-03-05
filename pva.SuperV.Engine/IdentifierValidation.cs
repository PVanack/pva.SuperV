using pva.SuperV.Engine.Exceptions;
using System.Text.RegularExpressions;

namespace pva.SuperV.Engine
{
    /// <summary>
    ///  Validates an identifier so that t can be used as a C# namespace (project), class (class) or property (field).
    /// </summary>
    internal static partial class IdentifierValidation
    {
        /// <summary>Regex for validating identifier name.</summary>
        [GeneratedRegex(Constants.IdentifierNamePattern)]
        private static partial Regex IdentifierNameRegex();

        /// <summary>
        /// Validates the identifier foe an entity.
        /// </summary>
        /// <param name="entityType">Type of entity.</param>
        /// <param name="identifier">Identifier to be validated.</param>
        /// <returns>Validated identifier.</returns>
        /// <exception cref="InvalidIdentifierNameException"></exception>
        public static string ValidateIdentifier(string entityType, string? identifier)
        {
            if (string.IsNullOrEmpty(identifier) || !IdentifierNameRegex().IsMatch(identifier))
            {
                throw new InvalidIdentifierNameException(entityType, identifier, Constants.IdentifierNamePattern);
            }
            return identifier;
        }
    }
}
