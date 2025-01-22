﻿using NSubstitute;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.EngineTests
{
    public class HistorizationProcessingTests
    {
        private const string ProcessingName = $"{ProjectHelpers.ClassName}_HIstorization";
        private const string ValueFieldName = "Value";

        private readonly WipProject project = Project.CreateProject(ProjectHelpers.ProjectName);
        private readonly IHistoryStorageEngine historyStorageEngine = Substitute.For<IHistoryStorageEngine>();
        private readonly Class clazz;
        private readonly IInstance instance = Substitute.For<IInstance>();
        private readonly Field<double> valueField = new(110.0);

        public HistorizationProcessingTests()
        {
            project.HistoryStorageEngine = historyStorageEngine;
            project.AddHistoryRepository(new HistoryRepository(ProjectHelpers.HistoryRepositoryName));
            clazz = new(ProjectHelpers.ClassName);
            clazz.FieldDefinitions.Add(ValueFieldName, new FieldDefinition<double>(ValueFieldName, 50));
            instance.GetField(ValueFieldName).Returns(valueField);
            instance.Name.Returns(ProjectHelpers.InstanceName);
        }

        [Fact]
        public void GivenHistorization_WhenChangingValue_ThenHistoryStorageEngineCreates()
        {
            // GIVEN
            List<string> fieldNamesToHistorize = [];
            fieldNamesToHistorize.Add(ValueFieldName);
            HistorizationProcessing<double> historizationProcessing = new(ProcessingName, project, clazz, ValueFieldName, ProjectHelpers.HistoryRepositoryName, null, fieldNamesToHistorize);
            DateTime valueTs = DateTime.Now;
            List<IField> actualHistorizedFields = [];
            historyStorageEngine
                .HistorizeValues(Arg.Any<string>(), ProjectHelpers.InstanceName, valueTs, Arg.Do<List<IField>>(arg => actualHistorizedFields = arg));

            // WHEN
            valueField.SetValue(100.0, valueTs);
            historizationProcessing.ProcessValue(instance, valueField, true, 50.0, valueField.Value);

            // THEN
            actualHistorizedFields.ShouldContain(valueField);
        }
    }
}