namespace pva.SuperV.Model.FieldDefinitions
{
    public record LongFieldDefinitionModel : FieldDefinitionModel
    {
        public LongFieldDefinitionModel(string Name) : base(Name, nameof(LongFieldDefinitionModel))
        {
        }
    }
}
