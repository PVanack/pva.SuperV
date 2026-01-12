using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Parser of script lines to identify field references.
    /// </summary>
    public static class ScriptParser
    {
        /// <summary>
        /// Parses the line, generating an array of lines and removing the blank lines.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>List of lines.</returns>
        public static List<string> ParseLine(string script)
        {
            var lines = new List<string>();
            using (var reader = new System.IO.StringReader(script))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0)
                    {
                        continue; // Skip empty lines
                    }

                    lines.AddRange([.. line.Split('\n')
                        .Where(l => l.Length > 0)]);
                }
            }
            return lines;
        }

        /// <summary>
        /// Parses the field references from the lines. a field reference is delimited with {{ and }}
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>List of references.</returns>
        public static List<FieldReference> ParseFieldReferences(List<string> lines)
        {
            List<FieldReference> fieldReferences = [];
            int lineIndex = 0;
            lines.ForEach(line =>
            {
                int startReferenceIndex = 0;
                int endReferenceIndex = -2;
                while ((startReferenceIndex = line.IndexOf("{{", endReferenceIndex + 2)) != -1)
                {
                    // Move past the "{{"
                    startReferenceIndex += 2;
                    endReferenceIndex = line.IndexOf("}}", startReferenceIndex);
                    if (endReferenceIndex == -1)
                    {
                        throw new ScriptSyntaxErrorException("Missing }}", line, startReferenceIndex);
                    }
                    string identifier = line.Substring(startReferenceIndex, endReferenceIndex - startReferenceIndex);
                    string[] parts = identifier.Split('.');
                    if (parts.Length == 0 || parts[0].IsWhiteSpace())
                    {
                        throw new ScriptSyntaxErrorException("Empty field reference", line, startReferenceIndex);
                    }
                    parts.ForEach(p => _ = IdentifierValidation.ValidateIdentifier("Field reference", p.Trim()));
                    FieldReference fieldReference;
                    if (parts.Length == 1)
                    {
                        fieldReference = new(null, parts[0].Trim());
                    }
                    else
                    {
                        fieldReference = new(parts[0].Trim(), parts[1].Trim());
                    }
                    fieldReferences.Add(fieldReference);
                }
                lineIndex++;
            });
            return fieldReferences;
        }

        /// <summary>
        /// Replaces the field references with actual variable names.
        /// </summary>
        /// <param name="ownInstance">The own instance for when the references don't have an instance.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="fieldReferences">The field references.</param>
        /// <returns>The lines of code with the references replaced with the variables></returns>
        public static List<string> ReplaceFieldReferences(string ownInstance, List<string> lines, List<FieldReference> fieldReferences)
        {
            List<string> processedLines = [];
            lines.ForEach(line =>
            {
                string processedLine = line;
                fieldReferences.ForEach(fieldReference =>
                {
                    string originalReference = fieldReference.GetOriginalString();
                    string replacementString = fieldReference.GetReplacementString(ownInstance);
                    processedLine = processedLine.Replace(originalReference, replacementString);
                });
                processedLines.Add(processedLine);
            });
            return processedLines;
        }
    }
}
