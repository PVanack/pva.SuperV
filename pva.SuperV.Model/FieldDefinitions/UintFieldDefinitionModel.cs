namespace pva.SuperV.Model.FieldDefinitions
{
    public record UintFieldDefinitionModel : FieldDefinitionModel
    {
        public UintFieldDefinitionModel(string Name) : base(Name, nameof(UintFieldDefinitionModel))
        {
        }
    }
}
