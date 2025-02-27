namespace pva.SuperV.Model.FieldDefinitions
{
    public record StringFieldDefinitionModel : FieldDefinitionModel
    {
        public StringFieldDefinitionModel(string Name) : base(Name, nameof(StringFieldDefinitionModel))
        {
        }
    }
}