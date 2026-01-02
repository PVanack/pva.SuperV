//#define DELETE_PROJECT_ASSEMBLY
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using pva.Helpers.Extensions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using pva.SuperV.TestContainers;

namespace pva.SuperV.EngineTests
{
    public class SuperVTestsBase : IAsyncDisposable
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
        protected const string BoolFieldName = "BoolField";
        protected const string DateTimeFieldName = "DateTimeField";
        protected const string DoubleFieldName = "DoubleField";
        protected const string FloatFieldName = "FloatField";
        protected const string IntFieldName = "IntField";
        protected const string LongFieldName = "LongField";
        protected const string ShortFieldName = "ShortField";
        protected const string StringFieldName = "StringField";
        protected const string TimeSpanFieldName = "TimeSpanField";
        protected const string UintFieldName = "UintField";
        protected const string UlongFieldName = "UlongField";
        protected const string UshortFieldName = "UshortField";
        protected const string IntFieldWithFormatName = "IntFieldWithFormat";

        protected static Dictionary<int, string> AlarmStatesFormatterValues { get; } = new() {
                { -2, "LowLow" },
                { -1, "Low" },
                { 0, "OK" },
                { 1, "High" },
                { 2, "HighHigh" }
            };

        private readonly TDengineContainer tdEngineContainer = new();
        protected ILoggerFactory LoggerFactory { get; } = new NLogLoggerFactory();

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            Project.Projects.Values.ForEach(project
                => project.Dispose());
            await tdEngineContainer.StopTDengineContainerAsync();
        }

        protected RunnableProject CreateRunnableProject()
        {
            return CreateRunnableProject(NullHistoryStorageEngine.Prefix);
        }

        public RunnableProject CreateRunnableProject(string? historyEngineType)
        {
            WipProject wipProject = CreateWipProject(historyEngineType);
            return Task.Run(async () => await Project.BuildAsync(wipProject)).Result;
        }

        protected WipProject CreateWipProject(string? historyEngineType)
        {
            string? connectionString;
            if (!String.IsNullOrEmpty(historyEngineType) && historyEngineType.Equals(TDengineHistoryStorage.Prefix))
            {
                string tdEngineConnectionString = Task.Run(async () => await tdEngineContainer.StartTDengineContainerAsync()).Result;
                connectionString = $"{TDengineHistoryStorage.Prefix}:{tdEngineConnectionString}";
            }
            else
            {
                connectionString = NullHistoryStorageEngine.Prefix;
            }

            WipProject wipProject = Project.CreateProject(ProjectName, connectionString);
            if (String.IsNullOrEmpty(historyEngineType))
            {
                wipProject.HistoryStorageEngine = new NullHistoryStorageEngine();
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
            AddFieldHistorization<bool>(wipProject, historyRepository, allFieldsClass, BoolFieldName);
            AddFieldHistorization<DateTime>(wipProject, historyRepository, allFieldsClass, DateTimeFieldName);
            AddFieldHistorization<double>(wipProject, historyRepository, allFieldsClass, DoubleFieldName);
            AddFieldHistorization<float>(wipProject, historyRepository, allFieldsClass, FloatFieldName);
            AddFieldHistorization<int>(wipProject, historyRepository, allFieldsClass, IntFieldName);
            AddFieldHistorization<long>(wipProject, historyRepository, allFieldsClass, LongFieldName);
            AddFieldHistorization<short>(wipProject, historyRepository, allFieldsClass, ShortFieldName);
            AddFieldHistorization<string>(wipProject, historyRepository, allFieldsClass, StringFieldName);
            AddFieldHistorization<TimeSpan>(wipProject, historyRepository, allFieldsClass, TimeSpanFieldName);
            AddFieldHistorization<uint>(wipProject, historyRepository, allFieldsClass, UintFieldName);
            AddFieldHistorization<ulong>(wipProject, historyRepository, allFieldsClass, UlongFieldName);
            AddFieldHistorization<ushort>(wipProject, historyRepository, allFieldsClass, UshortFieldName);
            AddFieldHistorization<int>(wipProject, historyRepository, allFieldsClass, IntFieldWithFormatName, AlarmStatesFormatterName);

            return wipProject;
        }

        private static void AddFieldHistorization<T>(WipProject wipProject, HistoryRepository historyRepository, Class allFieldsClass, string fieldName, string? fieldFormatName = null)
        {
            wipProject.AddField(AllFieldsClassName, new FieldDefinition<T>(fieldName, default), fieldFormatName);
            var historyProcessing = new HistorizationProcessing<T>($"H{fieldName}", wipProject, allFieldsClass, fieldName, historyRepository.Name, null,
                    [
                        fieldName,
                    ]
                );
            wipProject.AddFieldChangePostProcessing(AllFieldsClassName, fieldName,
                historyProcessing);
        }

        protected void DeleteProject(Project project)
        {
            _ = Task.Run(async () => await tdEngineContainer.StopTDengineContainerAsync()).Result;
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