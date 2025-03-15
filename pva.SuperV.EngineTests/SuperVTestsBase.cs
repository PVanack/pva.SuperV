//#define DELETE_PROJECT_ASSEMBLY
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using NSubstitute;
using pva.SuperV.Engine;
using pva.SuperV.Engine.FieldValueFormatters;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;

namespace pva.SuperV.EngineTests
{
    public class SuperVTestsBase : IDisposable
    {
        protected const string ProjectName = "TestProject";
        protected const string ClassName = "TestClass";
        protected const string InstanceName = "Instance";
        protected const string ValueFieldName = "Value";
        protected const string AlarmStateFieldName = "AlarmState";
        protected const string AlarmStatesFormatterName = "AlarmStates";
        protected const string HighHighLimitFieldName = "HighHighLimit";
        protected const string HighLimitFieldName = "HighLimit";
        protected const string LowLimitFieldName = "LowLimit";
        protected const string LowLowLimitFieldName = "LowLowlimit";
        protected const string BaseClassName = "TestBaseClass";
        protected const string BaseClassFieldName = "InheritedField";
        protected const string HistoryRepositoryName = "HistoryRepository";
        protected static Dictionary<int, string> AlarmStatesFormatterValues = new() {
                { -2, "LowLow" },
                { -1, "Low" },
                { 0, "OK" },
                { 1, "High" },
                { 2, "HighHigh" }
            };
        private IContainer? tdEngineContainer;

        public void Dispose()
        {
            Task.Run(async () => await StopTDengineContainerAsync()).Wait();
        }

        protected async Task<string> StartTDengineContainerAsync()
        {
            if (tdEngineContainer is null)
            {
                tdEngineContainer = new ContainerBuilder()
                    .WithImage("tdengine/tdengine:latest")
                    .WithPortBinding(6030, false)
                    .WithWaitStrategy(
                        Wait.ForUnixContainer()
                            .UntilPortIsAvailable(6030)
                            .UntilPortIsAvailable(6041)
                            .UntilPortIsAvailable(6043)
                            .UntilPortIsAvailable(6060)
                    )
                    .Build();

                // Start the container.
                await tdEngineContainer.StartAsync()
                  .ConfigureAwait(false);
                // Wait to make sure the processes in container are ready and running.
                Thread.Sleep(500);
            }
            return $"host={tdEngineContainer.Hostname};port={tdEngineContainer.GetMappedPublicPort(6030)};username=root;password=taosdata";
        }

        protected async Task StopTDengineContainerAsync()
        {
            if (tdEngineContainer is not null)
            {
                await tdEngineContainer.StopAsync()
                    .ConfigureAwait(false);
                long exitCode = await tdEngineContainer.GetExitCodeAsync();
                tdEngineContainer = null;
            }
        }

        protected RunnableProject CreateRunnableProject()
        {
            return CreateRunnableProject("");
        }

        public RunnableProject CreateRunnableProject(string? historyEngineType)
        {
            WipProject wipProject = CreateWipProject(historyEngineType);
            RunnableProject project = Task.Run(async () => await Project.BuildAsync(wipProject)).Result;
            return project;
        }

        protected WipProject CreateWipProject(string? historyEngineType)
        {
            string? connectionString = historyEngineType;
            if (!String.IsNullOrEmpty(historyEngineType) && historyEngineType.Equals(TDengineHistoryStorage.Prefix))
            {
                string tdEngineConnectionString = Task.Run(async () => await StartTDengineContainerAsync()).Result;
                connectionString = $"{TDengineHistoryStorage.Prefix}:{tdEngineConnectionString}";
            }

            WipProject wipProject = Project.CreateProject(ProjectName, connectionString);
            if (String.IsNullOrEmpty(historyEngineType))
            {
                IHistoryStorageEngine historyStorageEngine = Substitute.For<IHistoryStorageEngine>();
                wipProject.HistoryStorageEngine = historyStorageEngine;
            }
            HistoryRepository historyRepository = new(HistoryRepositoryName);
            wipProject.AddHistoryRepository(historyRepository);

            _ = wipProject.AddClass(BaseClassName);
            wipProject.AddField(BaseClassName, new FieldDefinition<string>(BaseClassFieldName, "InheritedField"));

            EnumFormatter formatter = new(AlarmStatesFormatterName, AlarmStatesFormatterValues);
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

            List<string> fieldsToHistorize = [ValueFieldName];
            HistorizationProcessing<int> historizationProcessing = new("Historization", wipProject, clazz, ValueFieldName, historyRepository.Name, null, fieldsToHistorize);
            wipProject.AddFieldChangePostProcessing(ClassName, ValueFieldName, historizationProcessing);
            return wipProject;
        }

        protected void DeleteProject(Project project)
        {
            Task.Run(async () => await StopTDengineContainerAsync());
#if DELETE_PROJECT_ASSEMBLY
            string projectAssemblyPath = project.GetAssemblyFileName();
            String projectName = project.Name!;
#endif
            Project.Unload(project);
#if DELETE_PROJECT_ASSEMBLY
            bool deleted = false;
            for (int i = 0; !deleted && i < 10; i++)
            {
                try
                {
                    File.Delete(projectAssemblyPath);
                    deleted = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(i * 100);
                }
            }
            if (!deleted)
            {
                Console.WriteLine($"Project {projectName} not deleted");
            }
#endif
        }
    }
}