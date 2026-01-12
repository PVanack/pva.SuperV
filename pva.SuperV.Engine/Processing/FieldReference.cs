

namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Field reference in a script.
    /// </summary>
    /// <param name="InstanceName">Name of instance. Can be null if the reference is for activating instance</param>
    /// <param name="FieldName">Name of the field</param>
    public record FieldReference(string? InstanceName, string FieldName)
    {
        public string GetReplacementString(string ownInstance) =>
            InstanceName is null ? $"{ownInstance}.{FieldName}.Value" : $"{InstanceName}.{FieldName}.Value";

        internal string GetOriginalString() =>
            "{{" + (InstanceName is null ? FieldName.Trim() : $"{InstanceName.Trim()}.{FieldName.Trim()}") + "}}";

    }
}
