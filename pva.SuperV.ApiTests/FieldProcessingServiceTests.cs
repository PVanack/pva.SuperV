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
            fieldProcessingService = new(LoggerFactory);
            runnableProject = CreateRunnableProject();
            wipProject = Project.CreateProject(runnableProject);
        }

        [Fact]
        public async Task GetFieldProcessings_ShouldReturnListOfFieldProcessings()
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
            List<FieldValueProcessingModel> FieldProcessings = await fieldProcessingService.GetFieldProcessingsAsync(runnableProject.GetId(), ClassName, ValueFieldName);

            // THEN
            FieldProcessings
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldProcessings);
        }

        [Fact]
        public async Task GetFieldProcessing_ShouldReturnFieldProcessing()
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
            FieldValueProcessingModel FieldProcessing = await fieldProcessingService.GetFieldProcessingAsync(runnableProject.GetId(), ClassName, ValueFieldName, expectedFieldProcessing.Name);

            // THEN
            FieldProcessing
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public async Task CreateAlarmStateFieldProcessing_ShouldCreateAlarmStateFieldProcessing()
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
            FieldValueProcessingModel FieldProcessing = await fieldProcessingService.CreateFieldProcessingAsync(wipProject.GetId(), ClassName, ValueFieldName, expectedFieldProcessing);

            // THEN
            FieldProcessing
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public async Task CreateHistorizationFieldProcessing_ShouldCreateHistorizationFieldProcessing()
        {
            // GIVEN
            FieldValueProcessingModel expectedFieldProcessing = new HistorizationProcessingModel("HistorizationAdded",
                    ValueFieldName,
                    HistoryRepositoryName,
                    null,
                    [
                        ValueFieldName,
                    ]);
            // WHEN
            FieldValueProcessingModel createdFieldProcessing = await fieldProcessingService.CreateFieldProcessingAsync(wipProject.GetId(), ClassName, ValueFieldName, expectedFieldProcessing);

            // THEN
            createdFieldProcessing
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public async Task UpdateHistorizationFieldProcessing_ShouldUpdateHistorizationFieldProcessing()
        {
            // GIVEN
            FieldValueProcessingModel expectedFieldProcessing = new HistorizationProcessingModel("HistorizationAdded",
                    ValueFieldName,
                    HistoryRepositoryName,
                    null,
                    [
                        ValueFieldName,
                    ]);
            _ = await fieldProcessingService.CreateFieldProcessingAsync(wipProject.GetId(), ClassName, ValueFieldName, expectedFieldProcessing);

            // WHEN
            expectedFieldProcessing = new HistorizationProcessingModel("HistorizationAdded",
                    ValueFieldName,
                    HistoryRepositoryName,
                    null,
                    [
                        ValueFieldName,
                        AlarmStateFieldName
                    ]);

            FieldValueProcessingModel updatedFieldProcessing = await fieldProcessingService.UpdateFieldProcessingAsync(wipProject.GetId(), ClassName, ValueFieldName,
                expectedFieldProcessing.Name, expectedFieldProcessing);

            // THEN
            updatedFieldProcessing
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldProcessing);
        }

        [Fact]
        public async Task DeleteFieldProcessing_ShouldDeleteFieldProcessing()
        {
            // GIVEN

            // WHEN
            await fieldProcessingService.DeleteFieldProcessingAsync(wipProject.GetId(), ClassName, ValueFieldName, "ValueAlarmState");

            // THEN
            wipProject.GetClass(ClassName)
                .GetField(ValueFieldName)
                .ValuePostChangeProcessings.FirstOrDefault(f => f.Name == "ValueAlarmState")
                .ShouldBeNull();
        }
    }
}
