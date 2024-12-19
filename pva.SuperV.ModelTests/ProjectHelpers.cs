using pva.SuperV.Model;

namespace pva.SuperV.ModelTests
{
    internal static class ProjectHelpers
    {
        public const string ProjectName = "TestProject";
        public const string ClassName = "TestClass";
        public const string InstanceName = "Instance";
        public const string FieldName = "IntField";

        public static RunnableProject CreateRunnableProject()
        {
            WipProject wipProject = Project.CreateProject(ProjectName);
            Class clazz = wipProject.AddClass(ClassName);
            clazz.AddField(new FieldDefinition<int>(FieldName, 10));
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
