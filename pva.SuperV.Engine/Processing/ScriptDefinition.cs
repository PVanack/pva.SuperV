using pva.Helpers.Extensions;
using System.Text;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Script definition used in a <see cref="Project"/>.
    /// </summary>
    public class ScriptDefinition
    {
        /// <summary>
        /// Gets the name of the script.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }
        /// <summary>
        /// Gets the name of the topic on which the script registers.
        /// </summary>
        /// <value>
        /// The name of the topic.
        /// </value>
        public string TopicName { get; }
        /// <summary>
        /// Gets the source code of the script.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get; }
        /// <summary>
        /// The field references used in the script.
        /// </summary>
        [JsonIgnore]
        public List<FieldReference> fieldReferences;
        /// <summary>
        /// The parsed lines
        /// </summary>
        [JsonIgnore]
        public List<string> lines;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="topicName">Name of the topicon which the script registers.</param>
        /// <param name="source">The source code of script.</param>
        public ScriptDefinition(string name, string topicName, string source)
        {
            Name = IdentifierValidation.ValidateIdentifier("script", name);
            TopicName = topicName;
            Source = source;
            lines = ScriptParser.ParseLine(Source);
            fieldReferences = ScriptParser.ParseFieldReferences(lines);
        }

        /// <summary>
        /// Gets the code to be used when building project.
        /// </summary>
        /// <returns>The code for the script.</returns>
        public string GetCode()
        {
            List<string> replacedScriptCode = ScriptParser.ReplaceFieldReferences(ScriptBase.ChangedInstance, lines, fieldReferences);
            StringBuilder generatedCode = new();
            HashSet<string> instanceReferences = [.. fieldReferences.Select(f => f.InstanceName ?? ScriptBase.ChangedInstance)];
            generatedCode.AppendLine($"public class {Name}Class : ScriptBase")
                .AppendLine("{")
                .AppendLine($"  public {Name}Class(RunnableProject project, ScriptDefinition scriptDefinition): base(project, scriptDefinition)")
                .AppendLine("  {")
                .AppendLine("  }")
                .AppendLine("public override void HandleFieldValueChange(RunnableProject project, Dictionary<string, IInstance> instances, FieldValueChangedEvent fieldValueChangedEvent)")
                .AppendLine("  {");
            instanceReferences.ForEach(instance =>
                generatedCode.AppendLine($"    var {instance} = instances[\"{instance}\"] as dynamic;"));
            replacedScriptCode.ForEach(line =>
                generatedCode.AppendLine(line));
            generatedCode.AppendLine("  }")
                .AppendLine("}");
            return generatedCode.ToString();
        }
    }
}
