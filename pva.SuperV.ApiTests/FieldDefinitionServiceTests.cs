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
                    new UshortFieldDefinitionModel("UshortField", default, null)
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
