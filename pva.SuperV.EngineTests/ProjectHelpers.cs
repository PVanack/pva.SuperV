﻿using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;

namespace pva.SuperV.EngineTests
{
    internal static class ProjectHelpers
    {
        public const string ProjectName = "TestProject";
        public const string ClassName = "TestClass";
        public const string InstanceName = "Instance";
        public const string ValueFieldName = "Value";
        public const string AlarmStateFieldName = "AlarmState";
        public const string AlarmStatesFormatterName = "AlarmStates";
        private const string HighHighLimitFieldName = "HighHighLimit";
        private const string HighLimitFieldName = "HighLimit";
        private const string LowLimitFieldName = "LowLimit";
        private const string LowLowLimitFieldName = "LowLowlimit";

        public const string BaseClassName = "TestBaseClass";
        public const string BaseClassFieldName = "InheritedField";
        public const string HistoryRepositoryName = "HistoryRepository";

        private static IContainer? tdEngineContainer;

        public static async Task<string> StartTDengineContainer()
        {
            tdEngineContainer = new ContainerBuilder()
                .WithImage("tdengine/tdengine:latest")
                .WithPortBinding(6030, false)
                .WithPortBinding(6031, false)
                .WithPortBinding(6032, false)
                .WithPortBinding(6033, false)
                .WithPortBinding(6034, false)
                .WithPortBinding(6035, false)
                .WithPortBinding(6036, false)
                .WithPortBinding(6037, false)
                .WithPortBinding(6038, false)
                .WithPortBinding(6039, false)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6030))
                .Build();

            // Start the container.
            await tdEngineContainer.StartAsync()
              .ConfigureAwait(false);
            return $"host={tdEngineContainer.Hostname};port={tdEngineContainer.GetMappedPublicPort(6030)};username=root;password=taosdata";
        }

        public static async Task StopTDengineContainer()
        {
            if (tdEngineContainer != null)
            {
                await tdEngineContainer.StopAsync()
                    .ConfigureAwait(false);
            }
        }

        public static RunnableProject CreateRunnableProject()
        {
            return CreateRunnableProject("");
        }

        public static RunnableProject CreateRunnableProject(string? historyEngineType)
        {
            string? connectionString = historyEngineType;
            if (!String.IsNullOrEmpty(historyEngineType) && historyEngineType.Equals(HistoryStorageEngineFactory.TdEngineHistoryStorage))
            {
                string tdEngineConnectionString = Task.Run(async () => await StartTDengineContainer()).Result;
                connectionString = $"{HistoryStorageEngineFactory.TdEngineHistoryStorage}:{tdEngineConnectionString}";
            }

            WipProject wipProject = Project.CreateProject(ProjectName, connectionString);
            if (String.IsNullOrEmpty(historyEngineType))
            {
                IHistoryStorageEngine historyStorageEngine = Substitute.For<IHistoryStorageEngine>();
                wipProject.HistoryStorageEngine = historyStorageEngine;
            }
            HistoryRepository historyRepository = new(ProjectHelpers.HistoryRepositoryName);
            wipProject.AddHistoryRepository(historyRepository);

            _ = wipProject.AddClass(BaseClassName);
            wipProject.AddField(BaseClassName, new FieldDefinition<string>(BaseClassFieldName, "InheritedField"));

            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, new Dictionary<int, string>{
                { -2, "LowLow" },
                { -1, "Low" },
                { 0, "OK" },
                { 1, "High" },
                { 2, "HighHigh" }
            });
            wipProject.AddFieldFormatter(formatter);
            Class clazz = wipProject.AddClass(ClassName, BaseClassName);
            wipProject.AddField(ClassName, new FieldDefinition<int>(ValueFieldName, 10));
            wipProject.AddField(ClassName, new FieldDefinition<int>(HighHighLimitFieldName, 100));
            wipProject.AddField(ClassName, new FieldDefinition<int>(HighLimitFieldName, 90));
            wipProject.AddField(ClassName, new FieldDefinition<int>(LowLimitFieldName, 10));
            wipProject.AddField(ClassName, new FieldDefinition<int>(LowLowLimitFieldName, 0));
            wipProject.AddField(ClassName, new FieldDefinition<int>(AlarmStateFieldName, 1), AlarmStatesFormatterName);

            AlarmStateProcessing<int> alarmStateProcessing = new("ValueAlarmState", clazz, ValueFieldName,
                HighHighLimitFieldName, HighLimitFieldName, LowLimitFieldName, LowLowLimitFieldName, null, AlarmStateFieldName, null);
            wipProject.AddFieldChangePostProcessing(ClassName, ValueFieldName, alarmStateProcessing);

            List<string> FieldsToHistorize = [ValueFieldName];
            HistorizationProcessing<int> historizationProcessing = new("Historization", wipProject, clazz, ValueFieldName, historyRepository.Name, null, FieldsToHistorize);
            wipProject.AddFieldChangePostProcessing(ClassName, ValueFieldName, historizationProcessing);

            RunnableProject project = ProjectBuilder.Build(wipProject);
            wipProject.Dispose();
            return project;
        }

        public static void DeleteProject(Project project)
        {
//            Task.Run(async () => await StopTDengineContainer());

            project.Dispose();
#if DELETE_PROJECT_FILE
            bool deleted = false;
            for (int i = 0; !deleted && i < 10; i++)
            {
                try
                {
                    File.Delete(project.GetAssemblyFileName());
                    deleted = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(i * 100);
                }
            }
            if (!deleted)
            {
                Console.WriteLine($"Project {project.Name} not deleted");
            }
#endif
        }
    }
}