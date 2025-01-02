namespace pva.SuperV.Model
{
    public interface IField
    {
        Type Type { get; }
        IFieldDefinition? FieldDefinition { get; set; }

        string? ToString();
    }
}