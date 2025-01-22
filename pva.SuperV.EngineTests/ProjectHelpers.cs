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

        public static RunnableProject CreateRunnableProject()
        {
            IHistoryStorageEngine historyStorageEngine = Substitute.For<IHistoryStorageEngine>();
            WipProject wipProject = Project.CreateProject(ProjectName);
            wipProject.HistoryStorageEngineConnectionString = "fake";
            wipProject.HistoryStorageEngine = historyStorageEngine;
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