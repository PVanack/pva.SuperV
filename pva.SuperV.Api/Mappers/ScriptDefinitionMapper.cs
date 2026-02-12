using pva.SuperV.Engine.Processing;
using pva.SuperV.Model.FieldProcessings;

namespace pva.SuperV.Api.Mappers
{
    public static class ScriptDefinitionMapper
    {
        public static ScriptDefinitionModel ToDto(ScriptDefinition scriptDefinition)
            => new(scriptDefinition.Name, scriptDefinition.TopicName, scriptDefinition.Source);

        public static ScriptDefinition FromDto(ScriptDefinitionModel scriptDefinitionModel)
            => new(scriptDefinitionModel.Name, scriptDefinitionModel.TopicName, scriptDefinitionModel.Source);
    }
}
