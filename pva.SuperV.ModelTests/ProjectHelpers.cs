using pva.SuperV.Model;

namespace pva.SuperV.ModelTests
{
    internal static class ProjectHelpers
    {
        public const string ProjectName = "TestProject";
        public const string ClassName = "TestClass";
        public const string InstanceName = "Instance";
        public const string IntFieldName = "IntField";
        public const string IntFieldWithEnumName = "IntWithEnumField";
        public const string EnumFormatterName = "ClosedOpened";

        public static RunnableProject CreateRunnableProject()
        {
            WipProject wipProject = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.EnumFormatterName, ["Closed", "Opened"]);
            wipProject.AddFieldFormatter(formatter);
            _ = wipProject.AddClass(ClassName);
            wipProject.AddField(ClassName, new FieldDefinition<int>(IntFieldName, 10));
            wipProject.AddField(ClassName, new FieldDefinition<int>(IntFieldWithEnumName, 1), EnumFormatterName);
            RunnableProject project = ProjectBuilder.Build(wipProject);
            wipProject.Dispose();
            return project;
        }

        public static void DeleteProject(Project project)
        {
            project.Dispose();
#if DELETE_PROJECT
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
#endif
        }
    }
}