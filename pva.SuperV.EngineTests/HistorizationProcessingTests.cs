using NSubstitute;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    public class HistorizationProcessingTests
    {
        private const string ProcessingName = "TestClass_HIstorization";
        private const string ValueFieldName = "Value";

        private readonly WipProject project = Project.CreateProject("TestProject");
        private readonly IHistoryStorageEngine historyStorageEngine = Substitute.For<IHistoryStorageEngine>();
        private readonly Class clazz;
        private readonly IInstance instance = Substitute.For<IInstance>();
        private readonly Field<double> valueField = new(110.0);

        public HistorizationProcessingTests()
        {
            project.HistoryStorageEngine = historyStorageEngine;
            project.AddHistoryRepository(new HistoryRepository("TestRepository"));
            clazz = new("TestClass");
            clazz.FieldDefinitions.Add(ValueFieldName, new FieldDefinition<double>(ValueFieldName, 50));
            instance.GetField(ValueFieldName).Returns(valueField);
            instance.Name.Returns("TestInstance");
        }

        [Fact]
        public void GivenHistorization_WhenChangingValue_ThenHistoryStorageEngineCreates()
        {
            // GIVEN
            List<string> fieldNamesToHistorize = [];
            fieldNamesToHistorize.Add(ValueFieldName);
            HistorizationProcessing<double> historizationProcessing = new(ProcessingName, project, clazz, ValueFieldName, "TestRepository", null,
                fieldNamesToHistorize);
            DateTime valueTs = DateTime.Now;
            List<IField> actualHistorizedFields = [];
            historyStorageEngine
                .HistorizeValues(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), "TestInstance", valueTs, QualityLevel.Good,
                Arg.Do<List<IField>>(arg => actualHistorizedFields = arg));

            // WHEN
            valueField.SetValue(100.0, valueTs, QualityLevel.Good);
            historizationProcessing.ProcessValue(instance, valueField, true, 50.0, valueField.Value);

            // THEN
            actualHistorizedFields.ShouldContain(valueField);
        }
    }
}