namespace pva.SuperV.Model.FieldDefinitions
{
    public record TimeSpanFieldDefinitionModel : FieldDefinitionModel
    {
        public TimeSpanFieldDefinitionModel(string Name) : base(Name, nameof(TimeSpanFieldDefinitionModel))
        {
        }
    }
}