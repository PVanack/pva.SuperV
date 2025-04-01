//#define DELETE_PROJECT_ASSEMBLY
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using pva.Helpers.Extensions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using System.Globalization;
using System.Net.NetworkInformation;

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
        protected const string AllFieldsClassName = "AllFieldsClass";
        protected const string AllFieldsInstanceName = "AllFieldsInstance";
        protected const string HistoryRepositoryName = "HistoryRepository";
        protected static Dictionary<int, string> AlarmStatesFormatterValues { get; } = new() {
                { -2, "LowLow" },
                { -1, "Low" },
                { 0, "OK" },
                { 1, "High" },
                { 2, "HighHigh" }
            };
        private IContainer? tdEngineContainer;

        public void Dispose()
        {
            Project.Projects.Values.ForEach(project
                => project.Dispose());
            _ = Task.Run(async () => await StopTDengineContainerAsync()).Result;
            GC.SuppressFinalize(this);
        }

        private static void WaitForPort(int port)
        {
            const int MaxWaitIndex = 50;
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            int index = 0;
            while (index < MaxWaitIndex && ipGlobalProperties.GetActiveTcpConnections().Any(pr => pr.LocalEndPoint.Port == port && pr.State == TcpState.Established))
            {
                Thread.Sleep(100);
                index++;
            }
            if (index == MaxWaitIndex)
            {
                throw new ApplicationException($"Port 6030 is in use after {MaxWaitIndex * 100} msecs");
            }
        }

        protected async Task<string> StartTDengineContainerAsync()
        {
            if (tdEngineContainer is null)
            {
                WaitForPort(6030);
                tdEngineContainer = new ContainerBuilder()
                    .WithImage("tdengine/tdengine:3.3.6.0")
                    .WithPortBinding(6030, false)
                    .WithWaitStrategy(
                        Wait.ForUnixContainer()
                    //                            .UntilPortIsAvailable(6030, waitStrategy => waitStrategy.WithTimeout(TimeSpan.FromSeconds(15)))
                    //                            .UntilPortIsAvailable(6041)
                    //                            .UntilPortIsAvailable(6043)
                    //                            .UntilPortIsAvailable(6060)
                    )
                    .Build();

                // Start the container.
                try
                {
                    await tdEngineContainer.StartAsync()
                      .ConfigureAwait(false);
                    // Wait to make sure the processes in container are ready and running.
                    bool connected = false;
                    int index = 0;
                    while (!connected && index < 10)
                    {
                        SystemCommand.Run($"taos -k -h {tdEngineContainer.Hostname} -P 6030 ", out string output, out string error);
                        connected = !output.StartsWith("0: unavailable");
                        if (!connected)
                        {
                            Thread.Sleep(500);
                            index++;
                        }
                    }
                }
                catch (Exception)
                {
                    await StopTDengineContainerAsync();
                    throw;
                }
            }
            return $"host={tdEngineContainer.Hostname};port={tdEngineContainer.GetMappedPublicPort(6030)};username=root;password=taosdata";
        }

        protected async Task<long> StopTDengineContainerAsync()
        {
            if (tdEngineContainer is not null)
            {
                await tdEngineContainer.StopAsync()
                    .ConfigureAwait(false);
                long exitCode = await tdEngineContainer.GetExitCodeAsync();
                WaitForPort(6030);
                tdEngineContainer = null;
                return exitCode;
            }
            return 0;
        }

        protected RunnableProject CreateRunnableProject()
        {
            return CreateRunnableProject(NullHistoryStorageEngine.Prefix);
        }

        public RunnableProject CreateRunnableProject(string? historyEngineType)
        {
            WipProject wipProject = CreateWipProject(historyEngineType);
            RunnableProject project = Task.Run(async () => await Project.BuildAsync(wipProject)).Result;
            return project;
        }

        protected WipProject CreateWipProject(string? historyEngineType)
        {
            string? connectionString;
            if (!String.IsNullOrEmpty(historyEngineType) && historyEngineType.Equals(TDengineHistoryStorage.Prefix))
            {
                string tdEngineConnectionString = Task.Run(async () => await StartTDengineContainerAsync()).Result;
                connectionString = $"{TDengineHistoryStorage.Prefix}:{tdEngineConnectionString}";
            }
            else
            {
                connectionString = NullHistoryStorageEngine.Prefix;
            }

            WipProject wipProject = Project.CreateProject(ProjectName, connectionString);
            if (String.IsNullOrEmpty(historyEngineType))
            {
                IHistoryStorageEngine historyStorageEngine = new NullHistoryStorageEngine();
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

            Class allFieldsClass = wipProject.AddClass(AllFieldsClassName);
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<bool>("BoolField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<DateTime>("DateTimeField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<double>("DoubleField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<float>("FloatField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<int>("IntField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<long>("LongField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<short>("ShortField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<string>("StringField", ""));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<TimeSpan>("TimeSpanField", TimeSpan.FromDays(0)));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<uint>("UintField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<ulong>("UlongField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<ushort>("UshortField", default));
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<int>("IntFieldWithFormat", default), AlarmStatesFormatterName);
            wipProject.AddFieldChangePostProcessing(AllFieldsClassName, "IntFieldWithFormat",
                new HistorizationProcessing<int>("Historization", wipProject, allFieldsClass, "IntFieldWithFormat", historyRepository.Name, null,
                    [
                        "BoolField",
                        //"DateTimeField",
                        "DoubleField",
                        "FloatField",
                        "IntField",
                        "LongField",
                        "ShortField",
                        "StringField",
                        "TimeSpanField",
                        "UintField",
                        "UlongField",
                        "UshortField",
                        "IntFieldWithFormat"
                    ]
                ));
            return wipProject;
        }

        protected void DeleteProject(Project project)
        {
            _ = Task.Run(async () => await StopTDengineContainerAsync()).Result;
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

        protected static DateTime ParseDateTime(string fromString)
        {
            return DateTime.Parse(fromString, CultureInfo.InvariantCulture.DateTimeFormat).ToUniversalTime();
        }

        protected static TimeSpan ParseTimeSpan(string? intervalString)
        {
            ArgumentNullException.ThrowIfNull(intervalString);
            return TimeSpan.Parse(intervalString, CultureInfo.InvariantCulture.DateTimeFormat);
        }
    }
}