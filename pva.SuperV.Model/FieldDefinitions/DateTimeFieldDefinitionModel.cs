namespace pva.SuperV.Model.FieldDefinitions
{
    public record DateTimeFieldDefinitionModel : FieldDefinitionModel
    {
        public DateTimeFieldDefinitionModel(string Name) : base(Name, nameof(DateTimeFieldDefinitionModel))
        {
        }
    }
}