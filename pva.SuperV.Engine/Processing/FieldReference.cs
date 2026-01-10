

namespace pva.SuperV.Engine.Processing
{
    public record FieldReference(string? InstanceName, string FieldName)
    {
        public string GetReplacementString(string ownInstance) =>
            InstanceName is null ? $"{ownInstance}.{FieldName}" : $"{InstanceName}.{FieldName}";

        internal string GetOriginalString() =>
            "{{" + (InstanceName is null ? FieldName : $"{InstanceName}.{FieldName}") + "}}";

    }
}
