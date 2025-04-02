using pva.SuperV.Api.Exceptions;
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
        private readonly List<FieldDefinitionModel> allFieldsExpectedFieldDefinitions =
            [
                new BoolFieldDefinitionModel("BoolField", default, null),
                new DateTimeFieldDefinitionModel("DateTimeField", default, null),
                new DoubleFieldDefinitionModel("DoubleField", default, null),
                new FloatFieldDefinitionModel("FloatField", default, null),
                new IntFieldDefinitionModel("IntField", default, null),
                new LongFieldDefinitionModel("LongField", default, null),
                new ShortFieldDefinitionModel("ShortField", default, null),
                new StringFieldDefinitionModel("StringField", "", null),
                new TimeSpanFieldDefinitionModel("TimeSpanField", TimeSpan.FromDays(0), null),
                new UintFieldDefinitionModel("UintField", default, null),
                new UlongFieldDefinitionModel("UlongField", default, null),
                new UshortFieldDefinitionModel("UshortField", default, null),
                new IntFieldDefinitionModel("IntFieldWithFormat", default, "AlarmStates")
            ];

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

            // WHEN
            List<FieldDefinitionModel> fieldDefinitions = fieldDefinitionService.GetFields(runnableProject.GetId(), AllFieldsClassName);

            // THEN
            fieldDefinitions
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(allFieldsExpectedFieldDefinitions);
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
            List<FieldDefinitionModel> expectedFieldDefinitions = allFieldsExpectedFieldDefinitions;

            // WHEN
            List<FieldDefinitionModel> createdFieldDefinitions = fieldDefinitionService.CreateFields(wipProject.GetId(), BaseClassName, expectedFieldDefinitions);

            // THEN
            createdFieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions);
        }

        [Fact]
        public void UpdateClassFieldDefinition_ShouldUpdateClassFieldDefinition()
        {
            // GIVEN
            FieldDefinitionModel expectedFieldDefinition = new IntFieldDefinitionModel("IntFieldWithFormat", default, null);

            // WHEN
            FieldDefinitionModel updatedFieldDefinition = fieldDefinitionService.UpdateField(wipProject.GetId(), AllFieldsClassName, expectedFieldDefinition.Name, expectedFieldDefinition);

            // THEN
            updatedFieldDefinition.ShouldBeEquivalentTo(expectedFieldDefinition);
        }

        [Fact]
        public void UpdateClassFieldDefinitionChangingType_ShouldThrowException()
        {
            // GIVEN
            FieldDefinitionModel expectedFieldDefinition = new ShortFieldDefinitionModel("IntFieldWithFormat", default, null);

            // WHEN/THEN
            Assert.Throws<EntityPropertyNotChangeableException>(()
                => fieldDefinitionService.UpdateField(wipProject.GetId(), AllFieldsClassName, expectedFieldDefinition.Name, expectedFieldDefinition));
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
