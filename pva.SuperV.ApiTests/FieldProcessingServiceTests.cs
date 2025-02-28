using pva.SuperV.Api.Services.FieldProcessings;
using pva.SuperV.Engine;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.FieldProcessings;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class FieldProcessingServiceTests
    {
        private readonly FieldProcessingService fieldProcessingService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public FieldProcessingServiceTests()
        {
            fieldProcessingService = new();
            runnableProject = ProjectHelpers.CreateRunnableProject();
            wipProject = Project.CreateProject(runnableProject);
        }

        [Fact]
        public void GetFieldProcessings_ShouldReturnListOfFieldProcessings()
        {
            // GIVEN
            List<string> fieldsToHistorize = [ProjectHelpers.ValueFieldName];
            List<FieldValueProcessingModel> expectedFieldProcessings =
            [
                new AlarmStateProcessingModel("ValueAlarmState",
                    typeof(int).ToString(),
                    ProjectHelpers.ValueFieldName,
                    ProjectHelpers.HighHighLimitFieldName,
                    ProjectHelpers.HighLimitFieldName,
                    ProjectHelpers.LowLimitFieldName,
                    ProjectHelpers.LowLowLimitFieldName, null,
                    ProjectHelpers.AlarmStateFieldName,
                    null),
                new HistorizationProcessingModel("Historization",
                                    typeof(int).ToString(),
                    ProjectHelpers.ValueFieldName,
                     ProjectHelpers.HistoryRepositoryName,
                     null,
                     fieldsToHistorize)
                ];
            // WHEN
            List<FieldValueProcessingModel> FieldProcessings = fieldProcessingService.GetFieldProcessings(runnableProject.GetId(), ProjectHelpers.ClassName, ProjectHelpers.ValueFieldName);

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
                    typeof(int).ToString(),
                    ProjectHelpers.ValueFieldName,
                    ProjectHelpers.HighHighLimitFieldName,
                    ProjectHelpers.HighLimitFieldName,
                    ProjectHelpers.LowLimitFieldName,
                    ProjectHelpers.LowLowLimitFieldName, null,
                    ProjectHelpers.AlarmStateFieldName,
                    null);
            // WHEN
            FieldValueProcessingModel FieldProcessing = fieldProcessingService.GetFieldProcessing(runnableProject.GetId(), ProjectHelpers.ClassName, ProjectHelpers.ValueFieldName, expectedFieldProcessing.Name);

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
                    typeof(int).ToString(),
                    ProjectHelpers.ValueFieldName,
                    ProjectHelpers.HighHighLimitFieldName,
                    ProjectHelpers.HighLimitFieldName,
                    ProjectHelpers.LowLimitFieldName,
                    ProjectHelpers.LowLowLimitFieldName, null,
                    ProjectHelpers.AlarmStateFieldName,
                    null);
            // WHEN
            FieldValueProcessingModel FieldProcessing = fieldProcessingService.CreateFieldProcessing(wipProject.GetId(), ProjectHelpers.ClassName, ProjectHelpers.ValueFieldName, expectedFieldProcessing);

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
            fieldProcessingService.DeleteFieldProcessing(wipProject.GetId(), ProjectHelpers.ClassName, ProjectHelpers.ValueFieldName, "ValueAlarmState");

            // THEN
            wipProject.GetClass(ProjectHelpers.ClassName)
                .GetField(ProjectHelpers.ValueFieldName)
                .ValuePostChangeProcessings.FirstOrDefault(f => f.Name == "ValueAlarmState")
                .ShouldBeNull();
        }
    }
}
