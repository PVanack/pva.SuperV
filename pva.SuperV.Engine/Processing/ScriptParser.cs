using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Engine.Processing
{
    public static class ScriptParser
    {
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
                    parts.ForEach(p => _ = IdentifierValidation.ValidateIdentifier("Field reference", p));
                    FieldReference fieldReference;
                    if (parts.Length == 1)
                    {
                        fieldReference = new(null, parts[0]);
                    }
                    else
                    {
                        fieldReference = new(parts[0], parts[1]);
                    }
                    fieldReferences.Add(fieldReference);
                }
                lineIndex++;
            });
            return fieldReferences;
        }

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
