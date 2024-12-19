using FluentAssertions;
using pva.Helpers;
using pva.SuperV.Model;

namespace pva.SuperV.ModelTests
{

    [Collection("Project building")]
    public class ProjectStorageTest
    {

        [Fact]
        public void GivenProjectWithClassAndField_WhenSavingAndReloadingRunnableProjectDefinition_ThenReloadedProjectDefinitionIsSameAsSavedProject()
        {
            // GIVEN

            // WHEN
            RunnableProject project = ProjectHelpers.CreateRunnableProject();
            var instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            string filename = ProjectStorage.SaveProjectDefinition(project);

            RunnableProject loadedProject = ProjectStorage.LoadProjectDefinition<RunnableProject>(filename);

            // THEN
            CheckProjectProperties(project, loadedProject);
            CheckProjectClasses(project, loadedProject);

            instance.Dispose();
            instance = null;
            ProjectHelpers.DeleteProject(project);
        }

        [Fact]
        public void GivenProjectWithInstances_WhenSavingAndReloadingRunnableProjectSnapshot_ThenReloadedProjectInstancesAreSameAsSavedProject()
        {
            // GIVEN

            // WHEN
            RunnableProject project = ProjectHelpers.CreateRunnableProject();
            var instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            Field<int> intField = instance.GetField<int>(ProjectHelpers.FieldName);
            string filename = ProjectStorage.SaveProjectInstances(project);
            project.Instances.Clear();

            ProjectStorage.LoadProjectInstances(project, filename);

            // THEN
            project.Instances.Count.Should().Be(1);
            var loadedInstance = project.GetInstance(ProjectHelpers.InstanceName);
            loadedInstance.Should().NotBeSameAs(instance);
            loadedInstance.Name.Should().Be(instance.Name);
            loadedInstance.Class.Name.Should().Be(instance.Class.Name);
            loadedInstance.Fields.Count.Should().Be(1);
            Field<int> loadedField = loadedInstance.GetField<int>(ProjectHelpers.FieldName);
            loadedField.Value.Should().Be(intField.Value);

            instance.Dispose();
            instance = null;
            ProjectHelpers.DeleteProject(project);
        }

        private static void CheckProjectClasses(RunnableProject project, RunnableProject loadedProject)
        {
            project.Classes
                .ForEach((k, v) =>
                {
                    loadedProject.Classes.Should().ContainKey(k);
                    Class loadedClass = loadedProject.Classes.GetValueOrDefault(k);
                    loadedClass.Name.Should().Be(v.Name);
                    loadedClass.FieldDefinitions.Count.Should().Be(v.FieldDefinitions.Count);
                    CheckClassFieldDefinitions(v, loadedClass);
                });
        }

        private static void CheckClassFieldDefinitions(Class savedClass, Class loadedClass)
        {
            savedClass.FieldDefinitions.ForEach((k, v) =>
            {
                loadedClass.FieldDefinitions.Should().ContainKey(k);
                dynamic loadedFieldDefinition = loadedClass.FieldDefinitions.GetValueOrDefault(k);
                dynamic savedFieldDefinition = v;
                Assert.True(loadedFieldDefinition.Name.Equals(savedFieldDefinition.Name));
                Assert.True(loadedFieldDefinition.Type.Equals(savedFieldDefinition.Type));
                Assert.True(loadedFieldDefinition.DefaultValue.Equals(savedFieldDefinition.DefaultValue));
            });
        }

        private static void CheckProjectProperties(RunnableProject project, RunnableProject loadedProject)
        {
            loadedProject.Should().NotBeNull();
            loadedProject.Name.Should().Be(project.Name);
            loadedProject.Description.Should().Be(project.Description);
            loadedProject.Classes.Count.Should().Be(project.Classes.Count);
        }
    }
}