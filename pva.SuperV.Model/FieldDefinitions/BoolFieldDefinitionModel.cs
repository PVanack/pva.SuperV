namespace pva.SuperV.Model.FieldDefinitions
{
    public record BoolFieldDefinitionModel : FieldDefinitionModel
    {
        public BoolFieldDefinitionModel(string Name) : base(Name, nameof(BoolFieldDefinitionModel))
        {
        }
    }
}