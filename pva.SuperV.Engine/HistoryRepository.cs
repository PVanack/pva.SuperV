
using pva.SuperV.Engine.Exceptions;
using System.Text.RegularExpressions;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// History repository to store history of instance values.
    /// </summary>
    public partial class HistoryRepository(string name)
    {
        /// <summary>
        /// Regex used to validate the repository name.
        /// </summary>
        [GeneratedRegex(Constants.IdentifierNamePattern)]
        private static partial Regex HistoryRepositoryNameRegex();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = ValidateName(name);

        /// <summary>
        /// Validates the name of the history repository.
        /// </summary>
        /// <param name="name">The history repository name.</param>
        /// <returns></returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.InvalidHistoryRepositoryNameException"></exception>
        private static string ValidateName(string name)
        {
            if (!HistoryRepositoryNameRegex().IsMatch(name))
            {
                throw new InvalidHistoryRepositoryNameException(name, Constants.IdentifierNamePattern);
            }
            return name;
        }
    }
}