using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Engine;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.FieldProcessings;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class FieldProcessingServiceTests : SuperVTestsBase
    {
        private readonly FieldProcessingService fieldProcessingService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public FieldProcessingServiceTests()
        {
            fieldProcessingService = new();
            runnableProject = CreateRunnableProject();
            wipProject = Project.CreateProject(runnableProject);
        }

        [Fact]
        public void GetFieldProcessings_ShouldReturnListOfFieldProcessings()
        {
            // GIVEN
            List<string> fieldsToHistorize = [ValueFieldName];
            List<FieldValueProcessingModel> expectedFieldProcessings =
            [
                new AlarmStateProcessingModel("ValueAlarmState",
                    ValueFieldName,
                    HighHighLimitFieldName,
                    HighLimitFieldName,
                    LowLimitFieldName,
                    LowLowLimitFieldName, null,
                    AlarmStateFieldName,
                    null),
                new HistorizationProcessingModel("Historization",
                    ValueFieldName,
                     HistoryRepositoryName,
                     null,
                     fieldsToHistorize)
                ];
            // WHEN
            List<FieldValueProcessingModel> FieldProcessings = fieldProcessingService.GetFieldProcessings(runnableProject.GetId(), ClassName, ValueFieldName);

            // THEN
            FieldProcessings
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldProcessings);
        }

        [Fact]
        public void GetFieldProcessing_ShouldReturnFieldProcessing()
        {
            // GIVEN
            FieldValueProcessingModel expectedFieldProcessing = new AlarmStateProcessingModel("ValueAlarmState",
                    ValueFieldName,
                    HighHighLimitFieldName,
                    HighLimitFieldName,
                    LowLimitFieldName,
                    LowLowLimitFieldName, null,
                    AlarmStateFieldName,
                    null);
            // WHEN
            FieldValueProcessingModel FieldProcessing = fieldProcessingService.GetFieldProcessing(runnableProject.GetId(), ClassName, ValueFieldName, expectedFieldProcessing.Name);

            // THEN
            FieldProcessing
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public void CreateFieldProcessing_ShouldCreateFieldProcessing()
        {
            // GIVEN
            FieldValueProcessingModel expectedFieldProcessing = new AlarmStateProcessingModel("ValueAlarmStateAdded",
                    ValueFieldName,
                    HighHighLimitFieldName,
                    HighLimitFieldName,
                    LowLimitFieldName,
                    LowLowLimitFieldName, null,
                    AlarmStateFieldName,
                    null);
            // WHEN
            FieldValueProcessingModel FieldProcessing = fieldProcessingService.CreateFieldProcessing(wipProject.GetId(), ClassName, ValueFieldName, expectedFieldProcessing);

            // THEN
            FieldProcessing
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public void DeleteFieldProcessing_ShouldDeleteFieldProcessing()
        {
            // GIVEN

            // WHEN
            fieldProcessingService.DeleteFieldProcessing(wipProject.GetId(), ClassName, ValueFieldName, "ValueAlarmState");

            // THEN
            wipProject.GetClass(ClassName)
                .GetField(ValueFieldName)
                .ValuePostChangeProcessings.FirstOrDefault(f => f.Name == "ValueAlarmState")
                .ShouldBeNull();
        }
    }
}
