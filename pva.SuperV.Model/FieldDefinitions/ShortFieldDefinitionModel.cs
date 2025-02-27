namespace pva.SuperV.Model.FieldDefinitions
{
    public record ShortFieldDefinitionModel : FieldDefinitionModel
    {
        public ShortFieldDefinitionModel(string Name) : base(Name, nameof(ShortFieldDefinitionModel))
        {
        }
    }
}