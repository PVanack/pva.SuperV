namespace pva.SuperV.Model.FieldDefinitions
{
    public record DoubleFieldDefinitionModel : FieldDefinitionModel
    {
        public DoubleFieldDefinitionModel(string Name) : base(Name, nameof(DoubleFieldDefinitionModel))
        {
        }
    }
}