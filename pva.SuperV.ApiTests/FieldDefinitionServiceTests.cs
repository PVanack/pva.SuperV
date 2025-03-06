using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.FieldDefinitions;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class FieldDefinitionServiceTests
    {
        private readonly FieldDefinitionService fieldDefinitionService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public FieldDefinitionServiceTests()
        {
            fieldDefinitionService = new();
            runnableProject = ProjectHelpers.CreateRunnableProject();
            wipProject = Project.CreateProject(runnableProject);
        }

        [Fact]
        public void GetClassFieldDefinitions_ShouldReturnListOfClassFieldDefinitions()
        {
            // GIVEN
            List<FieldDefinitionModel> expectedFieldDefinitions = [new StringFieldDefinitionModel(ProjectHelpers.BaseClassFieldName, "InheritedField")];
            // WHEN
            List<FieldDefinitionModel> fieldDefinitions = fieldDefinitionService.GetFields(runnableProject.GetId(), ProjectHelpers.BaseClassName);

            // THEN
            fieldDefinitions
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldDefinitions);
        }

        [Fact]
        public void GetClassFieldDefinition_ShouldReturnClassFieldDefinition()
        {
            // GIVEN
            FieldDefinitionModel expectedFieldDefinition = new StringFieldDefinitionModel(ProjectHelpers.BaseClassFieldName, "InheritedField");
            // WHEN
            FieldDefinitionModel fieldDefinition = fieldDefinitionService.GetField(runnableProject.GetId(), ProjectHelpers.BaseClassName, ProjectHelpers.BaseClassFieldName);

            // THEN
            fieldDefinition
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldDefinition);
        }

        [Fact]
        public void CreateClassFieldDefinition_ShouldCreateClassFieldDefinition()
        {
            // GIVEN
            List<FieldDefinitionModel> expectedFieldDefinitions = [new StringFieldDefinitionModel($"{ProjectHelpers.BaseClassFieldName}Added", "")];
            // WHEN
            fieldDefinitionService.CreateFields(wipProject.GetId(), ProjectHelpers.BaseClassName, expectedFieldDefinitions);

            // THEN
        }

        [Fact]
        public void DeleteClassFieldDefinition_ShouldDeleteClassFieldDefinition()
        {
            // GIVEN

            // WHEN
            fieldDefinitionService.DeleteField(wipProject.GetId(), ProjectHelpers.BaseClassName, ProjectHelpers.BaseClassFieldName);

            // THEN
            wipProject.GetClass(ProjectHelpers.BaseClassName).FieldDefinitions.Keys.ShouldNotContain(ProjectHelpers.BaseClassFieldName);
        }
    }
}
