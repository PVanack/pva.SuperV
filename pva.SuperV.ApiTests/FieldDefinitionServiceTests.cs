using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.FieldDefinitions;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class FieldDefinitionServiceTests : SuperVTestsBase
    {
        private readonly FieldDefinitionService fieldDefinitionService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public FieldDefinitionServiceTests()
        {
            fieldDefinitionService = new();
            runnableProject = CreateRunnableProject();
            wipProject = Project.CreateProject(runnableProject);
        }

        [Fact]
        public void GetClassFieldDefinitions_ShouldReturnListOfClassFieldDefinitions()
        {
            // GIVEN
            List<FieldDefinitionModel> expectedFieldDefinitions =
                [
                    new StringFieldDefinitionModel(BaseClassFieldName, "InheritedField", null)
                ];
            // WHEN
            List<FieldDefinitionModel> fieldDefinitions = fieldDefinitionService.GetFields(runnableProject.GetId(), BaseClassName);

            // THEN
            fieldDefinitions
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldDefinitions);
        }

        [Fact]
        public void GetClassFieldDefinition_ShouldReturnClassFieldDefinition()
        {
            // GIVEN
            FieldDefinitionModel expectedFieldDefinition =
                new StringFieldDefinitionModel(BaseClassFieldName, "InheritedField", null);
            // WHEN
            FieldDefinitionModel fieldDefinition = fieldDefinitionService.GetField(runnableProject.GetId(), BaseClassName, BaseClassFieldName);

            // THEN
            fieldDefinition
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldDefinition);
        }

        [Fact]
        public void CreateClassFieldDefinition_ShouldCreateClassFieldDefinition()
        {
            // GIVEN
            List<FieldDefinitionModel> expectedFieldDefinitions =
                [
                new StringFieldDefinitionModel($"{BaseClassFieldName}Added", "", null)
                ];
            // WHEN
            List<FieldDefinitionModel> createdFieldDefinitions = fieldDefinitionService.CreateFields(wipProject.GetId(), BaseClassName, expectedFieldDefinitions);

            // THEN
            createdFieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions);
        }

        [Fact]
        public void DeleteClassFieldDefinition_ShouldDeleteClassFieldDefinition()
        {
            // GIVEN

            // WHEN
            fieldDefinitionService.DeleteField(wipProject.GetId(), BaseClassName, BaseClassFieldName);

            // THEN
            wipProject.GetClass(BaseClassName).FieldDefinitions.Keys.ShouldNotContain(BaseClassFieldName);
        }
    }
}
