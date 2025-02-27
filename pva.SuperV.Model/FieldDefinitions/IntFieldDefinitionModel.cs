namespace pva.SuperV.Model.FieldDefinitions
{
    public record IntFieldDefinitionModel : FieldDefinitionModel
    {
        public IntFieldDefinitionModel(string Name) : base(Name, nameof(IntFieldDefinitionModel))
        {
        }
    }
}
