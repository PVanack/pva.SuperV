namespace pva.SuperV.Model.FieldDefinitions
{
    public record FloatFieldDefinitionModel : FieldDefinitionModel
    {
        public FloatFieldDefinitionModel(string Name) : base(Name, nameof(FloatFieldDefinitionModel))
        {
        }
    }
}